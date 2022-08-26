using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Microsoft.VisualBasic;

namespace Animotion {
    [CreateAssetMenu(menuName = "Animotion/Animotion Tree")]
    public class TreeData : ScriptableObject {

        private string previousName;

        public NodeData root {
            get {
                return nodes.Find(n => n != null && n.isRoot);
            }
        }

#if UNITY_EDITOR
        public string folderPath {
            get {
                return GetFolderPath() + "/" + name;
            }
        }
#endif

        public List<NodeData> nodes {
            get {
                if (m_nodes == null) m_nodes = new List<NodeData>();
                return m_nodes;
            }
        }
        public List<NodeData> m_nodes;

        public Dictionary<NodeData, List<NodeData>> nodeAndChildren {
            get {
                Dictionary<NodeData, List<NodeData>> result = new Dictionary<NodeData, List<NodeData>>();

                foreach (NodeData parent in nodes) {
                    result.Add(parent, nodes.Where(nd => parent.children.Contains(nd.id)).ToList());
                }
                return result;
            }
        }

        public List<LinkData> links {
            get {
                if (m_links == null) m_links = new List<LinkData>();
                return m_links;
            }
        }
        public List<LinkData> m_links;

        public List<TreeProperty> propertyList {
            get {
                if (m_propertyList == null) m_propertyList = new List<TreeProperty>();
                return m_propertyList;
            }
        }
        public List<TreeProperty> m_propertyList;


        public NodeData GetNode(int id) {
            return nodes.Find(n => n.id == id);
        }

        public void SetRoot(NodeData nodeData) {
            if (root) root.isRoot = false;
            nodeData.isRoot = true;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void AddNode(NodeData node) {
            nodes.Add(node);
#if UNITY_EDITOR
            Debug.Log(Directory.Exists(folderPath));
            if (!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }
            AssetDatabase.CreateAsset(node, folderPath + "/node" + node.id + ".asset");
                EditorUtility.SetDirty(this);
#endif
        }

        public void DeleteNode(NodeData node) {
            nodes.Remove(node);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void AddLink(LinkData link) {
            links.Add(link);
#if UNITY_EDITOR
            if (!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }
            AssetDatabase.CreateAsset(link, folderPath + "/link" + link.startNodeId +"-" + link.endNodeId + ".asset");
            EditorUtility.SetDirty(this);
#endif
        }

        public void DeleteLink(LinkData link) {
            links.Remove(link);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }


        public void DeleteNode(int id) {
            DeleteNode(nodes.Find(node => node.id == id));
#if UNITY_EDITOR
            AssetDatabase.DeleteAsset(folderPath + "/node" + id + ".asset");
            EditorUtility.SetDirty(this);
#endif
        }

        public void Clear() {
            nodes.Clear();
            links.Clear();
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }


#if UNITY_EDITOR
        private string GetFolderPath() {
            string[] splitString = AssetDatabase.GetAssetPath(this).Split(new char[] { '/' });
            return String.Join("/", splitString.Take(splitString.Length - 1));
        }
#endif

        private void OnValidate() {
#if UNITY_EDITOR
            //Debug.Log("OnValidate : " + name + " previous : " + previousName);
            if (previousName != name && previousName != "") {
                string previousFolderPath = GetFolderPath() + "/" + previousName;
                string currentFolderPath = GetFolderPath() + "/" + name;
                //Debug.Log(previousFolderPath + "->" + currentFolderPath);
                AssetDatabase.MoveAsset(previousFolderPath, currentFolderPath);
                //AssetDatabase.Refresh();
        }
        previousName = name;
#endif
        }
    }
}