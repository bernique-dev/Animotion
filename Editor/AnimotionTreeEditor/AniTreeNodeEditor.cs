using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    [Serializable]
    public class AniTreeNodeEditor : AnimotionEditorWindowComponent {

        public AniNode node;
        public AniTreeEditor animotionTreeEditor;
        public AniTree tree;

        public bool isSelected;
        public bool isMoved;

        private Vector2 lastPosition;

        public bool isLinkBeingCreated;

        private Vector2 rectCenterOffset = Vector2.zero;

        public Rect rect {
            get {
                Vector2 rectangleSize = new Vector2(100, 35);
                return RectUtils.GetRect(new Vector2(Screen.width / 2, Screen.height / 2) + node.position + animotionTreeEditor.centerOffset, rectangleSize);
            }
        }

        private bool didRectContain;

        public void SetValues(AniNode _node, AniTreeEditor _animotionTreeEditor) {
            node = _node;
            animotionTreeEditor = _animotionTreeEditor;
            tree = _animotionTreeEditor.tree;
        }

        public override void Draw() {
            base.Draw();
            Handles.color = Color.white;
            Color backgroundColor = node.isRoot ? new Color32(0, 75, 0, 255) : AniTreeEditor.NODE_BACKGROUND_COLOR;
            Animotor animotionAnimator = animotionTreeEditor.animotor;
            if (Application.isPlaying && animotionAnimator != null) {
                if (animotionAnimator.currentNode) {
                    // Changes the color if GameObject with AnimotionAnimator selected  
                    backgroundColor = animotionTreeEditor.animotor.currentNode.id == node.id ? new Color32(125, 0, 0, 255) : backgroundColor;
                }
            }
            // Draws the node's rectangle
            Handles.DrawSolidRectangleWithOutline(rect, backgroundColor, isSelected ? Color.white : AniTreeEditor.BORDER_COLOR);
            if (animotionAnimator) {
                AniNode currentNode = animotionAnimator.currentNode;
                if (currentNode) {
                    if (node.id == currentNode.id) {
                        AniClip animotionClip = animotionAnimator.animotionClip;
                        if (animotionClip) {
                            Color translucentColor = Color.white;
                            // Draws the progression of the animator inside a state (GameObject with AnimotionAnimator needs to be selected)
                            Handles.DrawSolidRectangleWithOutline(new Rect(rect.min + new Vector2(1, rect.height - 1), new Vector2(rect.width * animotionTreeEditor.animotor.frame / animotionClip.length, rect.height / 100) - new Vector2(2, 2)), translucentColor, translucentColor);
                        }
                    }
                }
            }
            Handles.Label(rect.min, node.nodeName + (node.clip ? "\n" + node.clip.name : "") + (node.clipGroup ? "\n" + node.clipGroup.name : ""));

            if (isLinkBeingCreated) {
                Vector2[] startSidesCenter = new Vector2[] {
                    rect.GetTopSideCenter(),
                    rect.GetDownSideCenter()
                };
                Vector2[] endPoints = new Vector2[0];
                Vector2 start = rect.center;
                Vector2 end = Event.current.mousePosition;

                // Draws the arrow's stroke
                DrawingUtils.FindClosestSegment(startSidesCenter, endPoints, ref start, ref end);
                Handles.DrawAAPolyLine(new Vector3[] { start, end });
                // Draws the arrow's triangle
                Vector3[] arrowPoints = DrawingUtils.GetArrowSummitPoints(start, end, 10, 7.5f);
                Handles.DrawAAConvexPolygon(arrowPoints);
            }
        }

        public override void ProcessEvent(Event e) {
            base.ProcessEvent(e);
            var doesRectContain = rect.Contains(e.mousePosition);
            if (doesRectContain) {
                if (e.isMouse) {
                    // Selects a node
                    if (e.type == EventType.MouseDown && e.button == 0 && !isMoved) {
                        animotionTreeEditor.SelectNode(this, e.shift);
                        Selection.activeObject = node;
                    }
                    // Right click for parameters
                    if (e.type == EventType.ContextClick) {
                        animotionTreeEditor.SelectNode(this, e.shift || isSelected);
                        Selection.activeObject = node;
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Create link"), false, () => StartLinkCreation());
                        menu.AddItem(new GUIContent("Set Root"), false, () => {
                            tree.SetRoot(node);
                            EditorUtility.SetDirty(node);
                            EditorUtility.SetDirty(tree);
                            ;
                        });

                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Delete"), false, () => animotionTreeEditor.DeleteNode(node.id));
                        menu.ShowAsContext();
                    }
                }

                if (e.type == EventType.DragPerform || e.type == EventType.DragUpdated) {
                    //Object hovering on Window
                    foreach (var obj in DragAndDrop.objectReferences) {
                        if (obj is AniClipGroup) {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            DragAndDrop.AcceptDrag();
                            isSelected = true;
                        }
                    }
                    // Object dropped
                    if (e.type == EventType.DragPerform) {
                        foreach (var obj in DragAndDrop.objectReferences) {
                            if (obj is AniClipGroup) node.clipGroup = obj as AniClipGroup;
                        }
                        isSelected = false;
                        EditorUtility.SetDirty(tree);
                    }
                }
            }
            var center = new Vector2(Screen.width, Screen.height) / 2;
            if ((e.type == EventType.MouseDrag && e.button == 0) && isSelected) {
                if (!isMoved) {
                    didRectContain = doesRectContain;
                    rectCenterOffset = e.mousePosition - node.position - center - animotionTreeEditor.centerOffset;
                }
                isMoved = true;
            }
            // Handles the movement of the node
            if (isMoved) {
                lastPosition = node.position + animotionTreeEditor.centerOffset;
                Vector2 newPosition = e.mousePosition - center + animotionTreeEditor.centerOffset;
                if (didRectContain) animotionTreeEditor.MoveSelectedNodesWithDelta(newPosition - lastPosition - rectCenterOffset);
            }
            if (e.type == EventType.MouseUp) {
                isMoved = false;
                didRectContain = false;
                EditorUtility.SetDirty(node);
                EditorUtility.SetDirty(tree);
            }
        }

        public void StartLinkCreation() {
            isLinkBeingCreated = true;
        }

    }
}
