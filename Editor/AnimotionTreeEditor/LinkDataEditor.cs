using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Animotion {
    [CustomEditor(typeof(LinkData))]
    public class LinkDataEditor : Editor {

        protected GUIStyle centeredStyle;

        protected bool conditionsFoldout;

        public override void OnInspectorGUI() {
            serializedObject.Update();
            LinkData linkData = target as LinkData;

            GUIStyle _centeredStyle = new GUIStyle(GUI.skin.label);
            _centeredStyle.alignment = TextAnchor.MiddleCenter;
            centeredStyle = _centeredStyle;

            GUILayout.Label(linkData.startNodeId + " -> " + linkData.endNodeId, centeredStyle);

            for (int i = 0; i < linkData.reverseConditions.Count; i++) {
                if (linkData.reverseConditions[i] == null) {
                    //linkData.conditions[i] = ScriptableObject.CreateInstance<TreePropertyCondition>();
                }
                linkData.reverseConditions[i].tree = linkData.tree;
                linkData.reverseConditions[i].treePropertyCondition = linkData.reverseConditions[i];
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_conditions"));

            serializedObject.ApplyModifiedProperties();
        }

    }
}