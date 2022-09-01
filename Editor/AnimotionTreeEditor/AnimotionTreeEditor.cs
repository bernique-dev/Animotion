using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class AnimotionTreeEditor : AnimotionEditorWindowComponent {

        /// <summary>
        /// Interface's borders' color (Dark Mode only)
        /// </summary>
        public static readonly Color BORDER_COLOR = new Color32(30, 30, 30, 255);

        public static readonly Color NODE_BACKGROUND_COLOR = new Color32(56 / 2, 56 / 2, 56 * 2, 255);

        public static readonly Color BACKGROUND_COLOR = new Color32(56, 56, 56, 255);
        public static readonly Color LIGHT_BACKGROUND_COLOR = new Color32(75, 75, 75, 255);


        /// <summary>
        /// Menubar's buttons' width
        /// </summary>
        public static readonly float MENUBAR_BUTTON_WIDTH = 55f;
        /// <summary>
        /// Menubar's buttons' height
        /// </summary>
        public static readonly float MENUBAR_BUTTON_HEIGHT = 20;


        public Vector2 centerOffset;

        public TreeData tree;
        public List<AnimotionTreeNodeEditor> drawnNodes;
        public List<AnimotionTreeLinkEditor> drawnLinks;

        public List<int> selectedNodes;
        public List<AnimotionTreeLinkEditor> selectedLinks;

        private AnimotionTreePropertiesEditor propertiesEditor;

        public AnimotionAnimator animotionAnimator {
            get {
                if (activeGameObject) {
                    AnimotionAnimator aa = activeGameObject.GetComponent<AnimotionAnimator>();
                    return aa ? (aa.treeData == tree ? aa : null) : null;
                }
                return null;
            }
        }
        private GameObject activeGameObject;

        [MenuItem("Animotion/Animotion Tree Editor")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(AnimotionTreeEditor), false, "Animotion Tree Editor");
        }


        private void OnFocus() {
            Initiate();
        }

        private void OnEnable() {
            Initiate();
        }


        private void ModeChanged(PlayModeStateChange playModeState) {
            if (playModeState == PlayModeStateChange.EnteredEditMode) {
                OnEnable();
            }
        }


        /// <summary>
        /// Initiate drawn lits and properties Editor
        /// </summary>
        private void Initiate() {
            if (selectedNodes == null) selectedNodes = new List<int>();
            if (selectedLinks == null) selectedLinks = new List<AnimotionTreeLinkEditor>();
            if (drawnNodes == null) drawnNodes = new List<AnimotionTreeNodeEditor>();
            if (drawnLinks == null) drawnLinks = new List<AnimotionTreeLinkEditor>();
            if (propertiesEditor == null) {
                propertiesEditor = ScriptableObject.CreateInstance<AnimotionTreePropertiesEditor>();
                propertiesEditor.animotionTreeEditor = this;
            }
            try {
                EditorApplication.playModeStateChanged += ModeChanged;
            } catch {

            }
            if (tree) {
                NodeData.idCounter = tree.nodes.Count;
                DrawNodes();
                DrawLinks();
                foreach (AnimotionTreeNodeEditor node in drawnNodes) {
                    node.isLinkBeingCreated = false;
                }
            }
        }

        private void OnGUI() {
            Draw();
            ProcessEvent(Event.current);
            Repaint();
        }

        public override void Draw() {
            Handles.BeginGUI();

            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<AnimotionAnimator>()) {
                activeGameObject = Selection.activeGameObject;
            }


            DrawMenuBar();

            if (drawnNodes.Count > 0) {
                if (!drawnNodes[0]) {
                    OnEnable();
                }
            }

            string str = NodeData.idCounter + "\n";
            if (tree) {
                foreach (NodeData node in tree.nodes) {
                    if (node) {
                        str += node.nodeName 
                                + " (" + node.id + ")" 
                                + " (" + tree.nodeAndChildren[node].Count + ")\n";
                    }
                }
                foreach (LinkData link in tree.links) {
                    if (link) {
                        str += link.startNodeId + " " + (link is BidirectionalLinkData ? "<":"-") + "-> " + link.endNodeId + "\n";
                    }
                }
            }

            foreach (AnimotionTreeLinkEditor animotionLinkNode in drawnLinks) {
                animotionLinkNode.isSelected = selectedLinks.Contains(animotionLinkNode);
                animotionLinkNode.Draw();
                animotionLinkNode.ProcessEvent(Event.current);
            }
            foreach (AnimotionTreeNodeEditor animotionTreeNode in drawnNodes) {
                animotionTreeNode.isSelected = selectedNodes.Contains(animotionTreeNode.node.id);
                animotionTreeNode.Draw();
                animotionTreeNode.ProcessEvent(Event.current);
            }

            str += "Nodes: " + drawnNodes.Count + " | Links: " + drawnLinks.Count;

            Handles.Label(new Vector2(7 * Screen.width / 8, Screen.height / 8), str);
            Handles.EndGUI();


            propertiesEditor.ProcessEvent(Event.current);
            propertiesEditor.Draw();

        }

        /// <summary>
        /// Draws the menubar
        /// </summary>
        public void DrawMenuBar() {

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Clear nodes", EditorStyles.toolbarButton)) {
                drawnNodes.Clear();
                drawnLinks.Clear();
                tree.Clear();
                NodeData.idCounter = 0;
            }
            if (GUILayout.Button("Refresh display", EditorStyles.toolbarButton)) {
                Initiate();
            }
            GUILayout.FlexibleSpace();
            if (tree) {
                if (GUILayout.Button(tree.name, EditorStyles.toolbarButton)) {
                    Selection.activeObject = tree;
                }
                if (activeGameObject) {
                    if (GUILayout.Button(activeGameObject.name, EditorStyles.toolbarButton)) {
                        if (activeGameObject == Selection.activeGameObject) Selection.activeGameObject = null;
                        activeGameObject = null;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public override void ProcessEvent(Event e) {
            if (focusedWindow == this && mouseOverWindow == this) {
                if (e.isMouse) {
                    GUI.FocusControl(null);
                    // Right click for parameters
                    if (IsNotContained(e.mousePosition)) {
                        if (e.type == EventType.ContextClick) {
                            if (IsLinkBeingCreated()) {
                                foreach (AnimotionTreeNodeEditor node in drawnNodes) {
                                    node.isLinkBeingCreated = false;
                                }
                            } else {
                                GenericMenu menu = new GenericMenu();
                                menu.AddItem(new GUIContent("Create new state"), false, () => CreateNode(e.mousePosition));
                                menu.ShowAsContext();
                            }
                        }
                        // Unselects selected items
                        if (e.type == EventType.MouseDown && e.button == 0) {
                            Unselect();
                        }
                        // Moves the graph
                        if (e.type == EventType.MouseDrag && e.button == 2) {
                            centerOffset += e.delta;
                        }
                    }
                }
            }

            if (e.type == EventType.DragPerform || e.type == EventType.DragUpdated) {
                //Object hovering on Window
                foreach (var obj in DragAndDrop.objectReferences) {
                    if (obj is TreeData) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        DragAndDrop.AcceptDrag();
                    }
                }
                // Object dropped on timeline
                if (e.type == EventType.DragPerform) {
                    foreach (var obj in DragAndDrop.objectReferences) {
                        if (obj is TreeData) {
                            tree = obj as TreeData;
                        }
                    }
                    OnEnable();
                }
            }
        }

        /// <summary>
        /// Creates a node on mouse position
        /// </summary>
        /// <param name="mousePos">Mouse position</param>
        public void CreateNode(Vector2 mousePos) {
            NodeData newNode = ScriptableObject.CreateInstance<NodeData>();
            newNode.SetValues("node" + NodeData.idCounter, mousePos - new Vector2(Screen.width / 2, Screen.height / 2));
            tree.AddNode(newNode);
            if (!tree.root) tree.SetRoot(newNode);
            AnimotionTreeNodeEditor atne = ScriptableObject.CreateInstance<AnimotionTreeNodeEditor>();
            atne.SetValues(newNode, this);
            drawnNodes.Add(atne);
        }


        /// <summary>
        /// Delete node
        /// </summary>
        /// <param name="id">Node's id</param>
        public void DeleteNode(int id) {
            tree.DeleteNode(id);
            DrawNodes();
            DrawLinks();
        }

        /// <summary>
        /// Deletes link
        /// </summary>
        /// <param name="link">Link data</param>
        public void DeleteLink(LinkData link) {
            tree.DeleteLink(link);
            tree.GetNode(link.startNodeId).children.Remove(link.endNodeId);
            if (link is BidirectionalLinkData) tree.GetNode(link.endNodeId).children.Remove(link.startNodeId);
            DrawLinks();
            Unselect();
        }

        /// <summary>
        /// Selects node
        /// </summary>
        /// <param name="nodeId">Node's id</param>
        /// <param name="multipleSelect">Indicates other nodes can be selected in the same time</param>
        public void SelectNode(int nodeId, bool multipleSelect = false) {
            SelectNode(drawnNodes.Find(dN => dN.node.id == nodeId), multipleSelect);
        }

        /// <summary>
        /// Selects node
        /// </summary>
        /// <param name="node">Drawn node</param>
        /// <param name="multipleSelect">Indicates other nodes can be selected in the same time</param>
        public void SelectNode(AnimotionTreeNodeEditor node, bool multipleSelect = false) {
            selectedLinks.Clear();
            if (!multipleSelect || IsLinkBeingCreated()) {
                selectedNodes.Clear();
            }
            selectedNodes.Add(node.node.id);
            if (IsLinkBeingCreated()) {
                CreateLink(GetLinkCreationAuthor(), node);
            }
        }

        /// <summary>
        /// Creates a link
        /// </summary>
        /// <param name="start">Link's start</param>
        /// <param name="end">Link's end</param>
        public void CreateLink(AnimotionTreeNodeEditor start, AnimotionTreeNodeEditor end) {
            start.isLinkBeingCreated = false;
            start.node.children.Add(end.node.id);
            if (!tree.links.Find(l => l.startNodeId == start.node.id && l.endNodeId == end.node.id)) {
                LinkData reverseLinkData = tree.links.Find(l => l.startNodeId == end.node.id && l.endNodeId == start.node.id);
                LinkData linkData = reverseLinkData ? ScriptableObject.CreateInstance<BidirectionalLinkData>() : ScriptableObject.CreateInstance<LinkData>();
                linkData.tree = tree;
                if (reverseLinkData) {
                    BidirectionalLinkData bidirectionalLinkData = linkData as BidirectionalLinkData;
                    bidirectionalLinkData.reverseConditions = reverseLinkData.conditions;
                    bidirectionalLinkData.reverseConditions = linkData.conditions;
                    linkData.startNodeId = end.node.id;
                    linkData.endNodeId = start.node.id;
                    tree.DeleteLink(reverseLinkData);
                } else {
                    linkData.startNodeId = start.node.id;
                    linkData.endNodeId = end.node.id;
                }
                tree.AddLink(linkData);
            }

            DrawLinks();
            SelectNode(end);
        }

        /// <summary>
        /// Select link
        /// </summary>
        /// <param name="link">Drawn link</param>
        public void SelectLink(AnimotionTreeLinkEditor link) {
            selectedLinks.Clear();
            selectedNodes.Clear();
            selectedLinks.Add(link);
        }

        /// <summary>
        /// Draws link
        /// </summary>
        /// <param name="linkData">Link data</param>
        public void DrawLink(LinkData linkData) {
            AnimotionTreeLinkEditor atle = ScriptableObject.CreateInstance<AnimotionTreeLinkEditor>();
            atle.SetValues(linkData, this);
            drawnLinks.Add(atle);
        }

        /// <summary>
        /// Returns if a link is being created or not
        /// </summary>
        public bool IsLinkBeingCreated() {
            return GetLinkCreationAuthor();
        }

        /// <summary>
        /// If a link is being created, returns the start node, null otherwise.
        /// </summary>
        /// <returns></returns>
        public AnimotionTreeNodeEditor GetLinkCreationAuthor() {
            return drawnNodes.Find(node => node.isLinkBeingCreated);
        }

        /// <summary>
        /// Moves selected nodes
        /// </summary>
        /// <param name="delta">Movement's delta</param>
        public void MoveSelectedNodes(Vector2 delta) {
            foreach (AnimotionTreeNodeEditor node in drawnNodes.FindAll(dN => selectedNodes.Contains(dN.node.id))) {
                node.node.position += delta - centerOffset;
            }
        }

        /// <summary>
        /// Unselects selected nodes/links
        /// </summary>
        public void Unselect() {
            selectedNodes.Clear();
            selectedLinks.Clear();
            Selection.activeGameObject = null;
        }

        /// <summary>
        /// Draws nodes corresponding to the list of NodeData
        /// </summary>
        public void DrawNodes() {
            drawnNodes = new List<AnimotionTreeNodeEditor>();
            foreach (NodeData node in tree.nodes) {
                AnimotionTreeNodeEditor atne = ScriptableObject.CreateInstance<AnimotionTreeNodeEditor>();
                atne.SetValues(node, this);
                drawnNodes.Add(atne);
            }
        }

        /// <summary>
        /// Draws links corresponding to the list of LinkData
        /// </summary>
        public void DrawLinks() {
            drawnLinks = new List<AnimotionTreeLinkEditor>();
            foreach (LinkData linkData in tree.links) {
                DrawLink(linkData);
            }
        }

        /// <summary>
        /// Returns if the mouse doesn't hover a component
        /// </summary>
        /// <param name="position">Mouse position</param>
        public bool IsNotContained(Vector2 position) {
            return !IsContainedByNode(position) && !IsContainedByLink(position) && !IsContainedByProperties(position);
        }

        /// <summary>
        /// Returns if the mouse hovers a node
        /// </summary>
        /// <param name="position">Mouse position</param>
        public bool IsContainedByNode(Vector2 position) {
            return drawnNodes.Find(atne => atne.rect.Contains(position));
        }

        /// <summary>
        /// Returns if the mouse hovers a link
        /// </summary>
        /// <param name="position">Mouse position</param>
        public bool IsContainedByLink(Vector2 position) {
            return drawnLinks.Find(atle => atle.Contains(position));
        }

        /// <summary>
        /// Returns if the mouse hovers the properties menu
        /// </summary>
        /// <param name="position">Mouse position</param>
        public bool IsContainedByProperties(Vector2 position) {
            return propertiesEditor.rect.Contains(position);
        }
    }
}


