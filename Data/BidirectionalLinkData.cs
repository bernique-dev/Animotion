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

        //! Transfert des listes dans booleans
        public Dictionary<string, bool> reverseBooleans;


    }
}
