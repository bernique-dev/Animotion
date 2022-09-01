using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class LinkData : ScriptableObject {

        public int startNodeId;
        public int endNodeId;

        public List<TreePropertyCondition> conditions {
            get {
                if (m_conditions == null) m_conditions = new List<TreePropertyCondition>();
                return m_conditions;
            }
            set {
                m_conditions = value;
            }
        }
        public List<TreePropertyCondition> m_conditions;

        [HideInInspector] public TreeData tree;

        public virtual string ToString() {
            return tree ? (tree.GetNode(startNodeId).nodeName + " -> " + tree.GetNode(endNodeId).nodeName) : (startNodeId + "->" + endNodeId);
        }


    }
}
