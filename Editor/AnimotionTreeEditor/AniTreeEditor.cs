using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class AniTreeEditor : AnimotionEditorWindowComponent {

        /// <summary>
        /// Interface's borders' color (Dark Mode only)
        /// </summary>
        public static readonly Color BORDER_COLOR = new Color32(30, 30, 30, 255);

        public static readonly Color NODE_BACKGROUND_COLOR = new Color32(10, 10, 100, 255);
        public static readonly Color ROOT_NODE_BACKGROUND_COLOR = new Color32(10, 100, 10, 255);
        public static readonly Color ALL_NODE_BACKGROUND_COLOR = new Color32(150, 60, 10, 255);
        public static readonly Color CURRENT_NODE_BACKGROUND_COLOR = new Color32(100, 100, 100, 255);

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

        public static AniTreeEditor instance;

        public Vector2 centerOffset;

        public AniTree tree;
        public List<AniTreeNodeEditor> drawnNodes;
        public List<AniTreeLinkEditor> drawnLinks;

        public List<int> selectedNodes;
        public List<AniTreeLinkEditor> selectedLinks;

        private AniTreePropertiesEditor propertiesEditor;

        private bool isRefreshingPaths;

        public Animotor animotor {
            get {
                if (activeGameObject) {
                    Animotor aa = activeGameObject.GetComponent<Animotor>();
                    return aa ? (aa.aniTree == tree ? aa : null) : null;
                }
                return null;
            }
        }
        private GameObject activeGameObject;

        private int treeIndex;
        private List<string> paths;
        private List<string> pathsWithoutExtension;

        [MenuItem("Animotion/Animotion Tree Editor")]
        public static void ShowWindow() {
            GetWindow(typeof(AniTreeEditor), false, "Animotion Tree Editor");
        }
        public static void ShowWindow(AniTree aniTree) {
            GetWindow(typeof(AniTreeEditor), false, "Animotion Tree Editor");
            instance.tree = aniTree;
            instance.Initiate();
        }


        private void OnFocus() {
            Initiate();
        }

        private void OnEnable() {
            instance = this;
            Initiate();
            RefreshPaths();
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
            var startTime = DateTime.Now;
            if (selectedNodes == null) selectedNodes = new List<int>();
            if (selectedLinks == null) selectedLinks = new List<AniTreeLinkEditor>();
            if (drawnNodes == null) drawnNodes = new List<AniTreeNodeEditor>();
            if (drawnLinks == null) drawnLinks = new List<AniTreeLinkEditor>();
            if (propertiesEditor == null) {
                propertiesEditor = CreateInstance<AniTreePropertiesEditor>();
                propertiesEditor.animotionTreeEditor = this;
            }
            try {
                EditorApplication.playModeStateChanged += ModeChanged;
            }
            catch {

            }
            if (tree) {
                AniNode.idCounter = tree.GetNodes().Count;
                DrawNodes();
                DrawLinks();
                foreach (AniTreeNodeEditor node in drawnNodes) {
                    node.isLinkBeingCreated = false;
                }
            }
        }

        public void RefreshPaths() {
            isRefreshingPaths = true;
            paths = AssetDatabase.FindAssets("t: AniTree").ToList().Select(uuid => AssetDatabase.GUIDToAssetPath(uuid)).ToList();
            paths = paths.Where(p => p.Contains("Assets")).ToList();
            pathsWithoutExtension = paths.Select(a => a.Substring(6, a.Length - 6).Substring(1)).ToList();
            isRefreshingPaths = false;
        }

        private void OnGUI() {
            Draw();
            ProcessEvent(Event.current);
            Repaint();
        }

        public override void Draw() {
            Handles.BeginGUI();

            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<Animotor>()) {
                if (Selection.activeGameObject != activeGameObject) {
                    if (Selection.activeGameObject.GetComponent<Animotor>().aniTree == tree) {
                        activeGameObject = Selection.activeGameObject;
                    }
                    else {
                        if (Application.isPlaying) {
                            activeGameObject = Selection.activeGameObject;
                            tree = Selection.activeGameObject.GetComponent<Animotor>().aniTree;

                            var assetPath = AssetDatabase.GetAssetPath(tree.GetInstanceID()).Substring(7);
                            treeIndex = pathsWithoutExtension.ToList().IndexOf(assetPath);
                            Initiate();
                        }
                    }
                }
            }

            DrawMenuBar();

            if (drawnNodes.Count > 0) {
                if (!drawnNodes[0]) {
                    OnEnable();
                }
            }

            string str = AniNode.idCounter + "\n";
            if (tree) {
                var nodesAndChildren = tree.GetNodesAndChildren();
                foreach (AniNode node in tree.GetNodes()) {
                    if (node) {
                        str += node.nodeName
                                + " (" + node.id + ")"
                                + " (" + nodesAndChildren[node.id].Count + ")\n";
                    }
                }
                foreach (AniLink link in tree.GetLinks()) {
                    if (link) {
                        str += link.startNodeId + " " + (link is AniBidirectionalLink ? "<" : "-") + "-> " + link.endNodeId + "\n";
                    }
                }
            }

            foreach (AniTreeLinkEditor animotionLinkNode in drawnLinks) {
                animotionLinkNode.isSelected = selectedLinks.Contains(animotionLinkNode);
                animotionLinkNode.Draw();
                animotionLinkNode.ProcessEvent(Event.current);
            }
            foreach (AniTreeNodeEditor animotionTreeNode in drawnNodes) {
                animotionTreeNode.isSelected = selectedNodes.Contains(animotionTreeNode.node.id);
                animotionTreeNode.Draw();
                if (tree.AreNodesSelectable()) animotionTreeNode.ProcessEvent(Event.current);
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
            //if (GUILayout.Button("Clear nodes", EditorStyles.toolbarButton)) {
            //    drawnNodes.Clear();
            //    drawnLinks.Clear();
            //    tree.Clear();
            //    NodeData.idCounter = 0;
            //}
            //if (GUILayout.Button("Refresh display", EditorStyles.toolbarButton)) {
            //    Initiate();
            //}

            if (Application.isPlaying) {
                GUILayout.Label("Selection unavaible in Play Mode");
            }
            else {
                if (paths.Count <= 0) {
                    GUILayout.Label("No tree found in project");
                }
                else {
                    var previousTreeIndex = treeIndex;
                    if (isRefreshingPaths) {
                        GUILayout.Label("Refreshing paths");
                    }
                    else {
                        treeIndex = EditorGUILayout.Popup(treeIndex, pathsWithoutExtension.ToArray(), EditorStyles.toolbarDropDown);
                        if (previousTreeIndex != treeIndex) {
                            var tmpTree = AssetDatabase.LoadAssetAtPath<AniTree>(paths[treeIndex]);
                            tree = tmpTree;
                            Initiate();
                        }
                    }
                }
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
                                foreach (AniTreeNodeEditor node in drawnNodes) {
                                    node.isLinkBeingCreated = false;
                                }
                            }
                            else {
                                GenericMenu menu = new GenericMenu();
                                menu.AddItem(new GUIContent("Create new state"), false, () => CreateNode(e.mousePosition));
                                menu.AddItem(new GUIContent("Create new global state"), false, () => CreateNode(e.mousePosition, AniNode.NodeType.Global));
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
                    if (obj is AniTree) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        DragAndDrop.AcceptDrag();
                    }
                }
                // Object dropped on timeline
                if (e.type == EventType.DragPerform) {
                    foreach (var obj in DragAndDrop.objectReferences) {
                        if (obj is AniTree) {
                            tree = obj as AniTree;
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
        public void CreateNode(Vector2 mousePos, AniNode.NodeType nodeType = AniNode.NodeType.Default) {
            AniNode newNode = CreateInstance<AniNode>();
            newNode.SetValues("node" + AniNode.idCounter, mousePos - new Vector2(Screen.width / 2, Screen.height / 2), nodeType);
            tree.AddNode(newNode);
            if (!tree.GetRoot()) tree.SetRoot(newNode);
            AniTreeNodeEditor atne = CreateInstance<AniTreeNodeEditor>();
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
        public void DeleteLink(AniLink link) {
            tree.DeleteLink(link);
            tree.GetNode(link.startNodeId).children.Remove(link.endNodeId);
            if (link is AniBidirectionalLink) tree.GetNode(link.endNodeId).children.Remove(link.startNodeId);
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
        public void SelectNode(AniTreeNodeEditor node, bool multipleSelect = false) {
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
        public void CreateLink(AniTreeNodeEditor start, AniTreeNodeEditor end) {
            start.isLinkBeingCreated = false;
            start.node.children.Add(end.node.id);
            var links = tree.GetLinks();
            if (!links.Find(l => l.startNodeId == start.node.id && l.endNodeId == end.node.id)) {
                AniLink reverseLinkData = links.Find(l => l.startNodeId == end.node.id && l.endNodeId == start.node.id);
                AniLink linkData = reverseLinkData ? CreateInstance<AniBidirectionalLink>() : CreateInstance<AniLink>();
                linkData.tree = tree;
                if (reverseLinkData) {
                    AniBidirectionalLink bidirectionalLinkData = linkData as AniBidirectionalLink;
                    bidirectionalLinkData.conditions = reverseLinkData.conditions;
                    bidirectionalLinkData.reverseConditions = new List<TreePropertyCondition>();
                    linkData.startNodeId = end.node.id;
                    linkData.endNodeId = start.node.id;
                    tree.DeleteLink(reverseLinkData);
                }
                else {
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
        public void SelectLink(AniTreeLinkEditor link) {
            selectedLinks.Clear();
            selectedNodes.Clear();
            selectedLinks.Add(link);
        }

        /// <summary>
        /// Draws link
        /// </summary>
        /// <param name="linkData">Link data</param>
        public void DrawLink(AniLink linkData) {
            AniTreeLinkEditor atle = ScriptableObject.CreateInstance<AniTreeLinkEditor>();
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
        public AniTreeNodeEditor GetLinkCreationAuthor() {
            return drawnNodes.Find(node => node.isLinkBeingCreated);
        }

        /// <summary>
        /// Moves selected nodes
        /// </summary>
        /// <param name="delta">Movement's delta</param>
        public void MoveSelectedNodesWithDelta(Vector2 delta) {
            foreach (AniTreeNodeEditor node in drawnNodes.FindAll(dN => selectedNodes.Contains(dN.node.id))) {
                node.node.position += delta - centerOffset;
            }
        }

        /// <summary>
        /// Moves selected nodes
        /// </summary>
        /// <param name="newPosition">Primarily moved node's new position</param>
        public void MoveSelectedNodesWithPosition(Vector2 newPosition) {
            foreach (AniTreeNodeEditor node in drawnNodes.FindAll(dN => selectedNodes.Contains(dN.node.id))) {
                node.node.position += newPosition - node.node.position;
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
            drawnNodes = new List<AniTreeNodeEditor>();
            var variant = tree as AniTreeVariant;
            foreach (AniNode node in tree.GetNodes()) {
                AniTreeNodeEditor atne = CreateInstance<AniTreeNodeEditor>();
                atne.SetValues(node, this);
                drawnNodes.Add(atne);
            }
        }

        /// <summary>
        /// Draws links corresponding to the list of LinkData
        /// </summary>
        public void DrawLinks() {
            drawnLinks = new List<AniTreeLinkEditor>();
            foreach (AniLink linkData in tree.GetLinks()) {
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


