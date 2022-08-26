using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class LinkData : ScriptableObject {

        public int startNodeId;
        public int endNodeId;

        public List<TreePropertyCondition> reverseConditions {
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
        public List<TreeProperty> properties {
            get {
                return tree.properties;
            }
        }

        public void SaveConditions() {
            foreach(TreePropertyCondition condition in reverseConditions) {

                //AssetDatabase.CreateAsset(condition, tree.folderPath + "/condition" + condition.id + ".asset");
                //EditorUtility.SetDirty(this);
            }
        }

    }
}
