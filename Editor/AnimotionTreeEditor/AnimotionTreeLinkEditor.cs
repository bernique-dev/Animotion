using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Animotion {
    [Serializable]
    public class AnimotionTreeLinkEditor : AnimotionEditorWindowComponent {

        public AnimotionTreeEditor animotionTreeEditor;

        public AnimotionTreeNodeEditor startNode {
            get {
                return animotionTreeEditor.drawnNodes.Find(dN => dN.node.id == linkData.startNodeId);
            }
        }
        public AnimotionTreeNodeEditor endNode {
            get {
                return animotionTreeEditor.drawnNodes.Find(dN => dN.node.id == linkData.endNodeId);
            }
        }
        public bool bidirectional;

        private Vector2 start; 
        private Vector2 end;

        private LinkData linkData;

        public bool isSelected;

        public float detectionRectWidth = 5;
        private bool clickContained;

        public void SetValues(LinkData _linkData, AnimotionTreeEditor _animotionTreeEditor) {
            linkData = _linkData;
            animotionTreeEditor = _animotionTreeEditor;
        }


        public override void Draw() {
            base.Draw();
            if (startNode && endNode) {
                Handles.color = isSelected ? Color.cyan : Color.white;
                start = startNode.rect.center;
                end = endNode.rect.center;
                Vector2[] startSidesCenter = new Vector2[] {
                    startNode.rect.GetTopSideCenter(),
                    startNode.rect.GetDownSideCenter()
                };

                Vector2[] endSidesCenter = new Vector2[] {
                    endNode.rect.GetTopSideCenter(),
                    endNode.rect.GetDownSideCenter()
                };

                DrawingUtils.FindClosestSegment(startSidesCenter, endSidesCenter, ref start, ref end);
                Handles.DrawAAPolyLine(new Vector3[] { start, end });

                Vector3[] arrowPoints = DrawingUtils.GetArrowSummitPoints(start,end,10,7.5f);
                if (linkData is not BidirectionalLinkData) Handles.DrawAAConvexPolygon(arrowPoints);


            }
        }

        public override void ProcessEvent(Event e) {
            base.ProcessEvent(e);
            clickContained = Contains(e.mousePosition);
            if (Contains(e.mousePosition) && !animotionTreeEditor.IsContainedByNode(e.mousePosition)) {
                if (e.type == EventType.ContextClick) {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => animotionTreeEditor.DeleteLink(linkData));
                    menu.ShowAsContext();
                }

                if (e.type == EventType.MouseDown && e.button == 0) {
                    animotionTreeEditor.SelectLink(this);
                    Selection.activeObject = (UnityEngine.Object)linkData;
                }
            }
        }

        public bool Contains(Vector2 position) {
            float widthDotProduct = Mathf.Abs(Vector2.Dot(position - start, Vector2.Perpendicular((end - start).normalized)));
            float heightDotProduct = Vector2.Dot(position - start, (end - start).normalized);
            return widthDotProduct <= detectionRectWidth && heightDotProduct >= 0 && heightDotProduct <= Vector2.Distance(start, end);
        }

    }
}

