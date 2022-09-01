using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Animotion {
    [CustomEditor(typeof(LinkData))]
    public class LinkDataEditor : Editor {

        protected GUIStyle centeredStyle;

        public override void OnInspectorGUI() {
            //base.OnInspectorGUI();
            serializedObject.Update();
            LinkData linkData = target as LinkData;

            GUIStyle _centeredStyle = new GUIStyle(GUI.skin.label);
            _centeredStyle.alignment = TextAnchor.MiddleCenter;
            centeredStyle = _centeredStyle;

            GUILayout.Label(linkData.tree.GetNode(linkData.startNodeId).nodeName + " -> " + linkData.tree.GetNode(linkData.endNodeId).nodeName, centeredStyle);

            for (int i = 0; i < linkData.conditions.Count; i++) {
                if (linkData.conditions[i] == null) {
                    //linkData.conditions[i] = ScriptableObject.CreateInstance<TreePropertyCondition>();
                }
                linkData.conditions[i].tree = linkData.tree;
                linkData.conditions[i].treePropertyCondition = linkData.conditions[i];
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_conditions"));

            serializedObject.ApplyModifiedProperties();
        }

    }
}