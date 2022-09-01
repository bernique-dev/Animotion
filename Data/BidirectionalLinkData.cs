using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    [Serializable]
    public class BidirectionalLinkData : LinkData {

        public List<TreePropertyCondition> reverseConditions {
            get {
                if (m_reverseConditions == null) m_reverseConditions = new List<TreePropertyCondition>();
                return m_reverseConditions;
            }
            set {
                m_reverseConditions = value;
            }
        }
        public List<TreePropertyCondition> m_reverseConditions;


        public override string ToString() {
            return tree ? (tree.GetNode(startNodeId).nodeName + " -- " + tree.GetNode(endNodeId).nodeName) : (startNodeId + "--" + endNodeId);
        }

    }
}
