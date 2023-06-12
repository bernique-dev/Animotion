using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Microsoft.VisualBasic;

namespace Animotion {
    [CreateAssetMenu(menuName = "Animotion/Animotion Tree")]
    public class AniTree : ScriptableObject {

        private string previousName;

        private AniNode root {
            get {
                return nodes.Find(n => n != null && n.isRoot);
            }
        }

#if UNITY_EDITOR
        private string folderPath {
            get {
                return GetFolderPath() + "/" + name;
            }
        }
#endif

        private List<AniNode> nodes {
            get {
                if (m_nodes == null) m_nodes = new List<AniNode>();
                return m_nodes;
            }
        }
        private List<AniNode> m_nodes;

        private Dictionary<int, List<AniNode>> nodesAndChildren {
            get {
                Dictionary<int, List<AniNode>> result = new Dictionary<int, List<AniNode>>();

                foreach (AniNode parent in nodes) {
                    result.Add(parent.id, nodes.Where(nd => parent.children.Contains(nd.id)).ToList());
                }
                return result;
            }
        }

        private List<AniLink> links {
            get {
                if (m_links == null) m_links = new List<AniLink>();
                return m_links;
            }
        }
        private List<AniLink> m_links;

        private List<TreeProperty> properties {
            get {
                if (m_properties == null) m_properties = new List<TreeProperty>();
                return m_properties;
            }
        }
        [SerializeField] private List<TreeProperty> m_properties;


        public virtual bool AreNodesSelectable() {
            return true;
        }

        public virtual AniNode GetRoot() {
            return root;
        }

        public virtual List<AniNode> GetNodes() {
            return nodes;
        }

        public virtual Dictionary<int, List<AniNode>> GetNodesAndChildren() {
            return nodesAndChildren;
        }

        public virtual List<AniLink> GetLinks() {
            return links;
        }

        public virtual AniNode GetNode(int id) {
            return nodes.Find(n => n.id == id);
        }

        public virtual List<TreeProperty> GetProperties() {
            return properties;
        }

        public void SetRoot(AniNode nodeData) {
            if (root) root.isRoot = false;
            nodeData.isRoot = true;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void AddNode(AniNode node) {
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

        public void DeleteNode(AniNode node) {
            nodes.Remove(node);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void AddLink(AniLink link) {
            links.Add(link);
#if UNITY_EDITOR
            if (!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }
            AssetDatabase.CreateAsset(link, folderPath + "/link" + link.startNodeId +"-" + link.endNodeId + ".asset");
            EditorUtility.SetDirty(this);
#endif
        }

        public void DeleteLink(AniLink link) {
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

        public void AddProperty(TreeProperty property) {
            properties.Add(property);
#if UNITY_EDITOR
            //Debug.Log(Directory.Exists(folderPath));
            if (!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }
            AssetDatabase.CreateAsset(property, folderPath + "/property" + property.id + ".asset");
            EditorUtility.SetDirty(this);
#endif
        }

        public void DeleteProperty(int id) {
            DeleteProperty(properties.Find(p => p.id == id));
#if UNITY_EDITOR
            AssetDatabase.DeleteAsset(folderPath + "/property" + id + ".asset");
            EditorUtility.SetDirty(this);
#endif
        }

        public void DeleteProperty(TreeProperty property) {
            properties.Remove(property);
#if UNITY_EDITOR
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