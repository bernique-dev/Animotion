﻿using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Animotion {
    [CreateAssetMenu(menuName = "Animotion/Animotion Tree")]
    public class TreeData : ScriptableObject {

        public NodeData root {
            get {
                return nodes.Find(n => n != null && n.isRoot);
            }
        }

        public List<string> serializedNodes {
            get {
                if (m_serializedNodes == null) m_serializedNodes = new List<string>();
                return m_serializedNodes;
            }
        }
        public List<string> m_serializedNodes;


        public List<string> serializedLinks {
            get {
                if (m_serializedLinks == null) m_serializedLinks = new List<string>();
                return m_serializedLinks;
            }
        }
        public List<string> m_serializedLinks;

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

        public List<string> booleanList;


        public void Serialize() {
            serializedNodes.Clear();
            serializedLinks.Clear();
            foreach (NodeData nodeData in nodes) {
#if UNITY_EDITOR
                nodeData.SaveData();
#endif
                serializedNodes.Add(JsonUtility.ToJson(nodeData));
            }
            foreach (LinkData linkData in links) {
                serializedLinks.Add(JsonUtility.ToJson(linkData));
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }


        public void Unserialize() {
            nodes.Clear();
            links.Clear();
            foreach (string serializedNode in serializedNodes) {
                NodeData nodeData = ScriptableObject.CreateInstance<NodeData>();
                JsonUtility.FromJsonOverwrite(serializedNode, nodeData);
#if UNITY_EDITOR
                if (nodeData.animotionClipsDataPath != "") {
                    nodeData.animotionClipsData = AssetDatabase.LoadAssetAtPath<AnimotionClipsData>(nodeData.animotionClipsDataPath);
                }
                if (nodeData.animotionClipPath != "") {
                    nodeData.animotionClip = AssetDatabase.LoadAssetAtPath<AnimotionClip>(nodeData.animotionClipPath);
                }
#endif
                nodes.Add(nodeData);
            }
            foreach (string serializedLink in serializedLinks) {
                LinkData linkData = serializedLink.Contains("reverse") ? ScriptableObject.CreateInstance<BidirectionalLinkData>() : ScriptableObject.CreateInstance<LinkData>();
                JsonUtility.FromJsonOverwrite(serializedLink, linkData);
                links.Add(linkData);
            }
#if UNITY_EDITOR
           EditorUtility.SetDirty(this);
#endif
        }

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
            EditorUtility.SetDirty(this);
#endif
        }

        public void DeleteNode(NodeData node) {
            nodes.Remove(node);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        public void DeleteNode(int id) {
            DeleteNode(nodes.Find(node => node.id == id));
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
    }
}