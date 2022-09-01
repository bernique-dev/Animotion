using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    [CustomEditor(typeof(BidirectionalLinkData))]
    public class BidirectionalLinkDataEditor : LinkDataEditor {

        public override void OnInspectorGUI() {
            BidirectionalLinkData biLinkData = target as BidirectionalLinkData;
            base.OnInspectorGUI();
            GUILayout.Label(biLinkData.tree.GetNode(biLinkData.endNodeId).nodeName + " -> " + biLinkData.tree.GetNode(biLinkData.startNodeId).nodeName, centeredStyle);

            for (int i = 0; i < biLinkData.reverseConditions.Count; i++) {
                if (biLinkData.reverseConditions[i] == null) {
                    //linkData.conditions[i] = ScriptableObject.CreateInstance<TreePropertyCondition>();
                }
                biLinkData.reverseConditions[i].tree = biLinkData.tree;
                biLinkData.reverseConditions[i].treePropertyCondition = biLinkData.reverseConditions[i];
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_reverseConditions"));

            serializedObject.ApplyModifiedProperties();
        }

    }

}