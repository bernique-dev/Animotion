using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Animotion {
    [CustomEditor(typeof(Animotor), true)]
    public class AnimotorInspector : Editor {

        public override void OnInspectorGUI() {

            EditorGUI.BeginChangeCheck();
            //base.OnInspectorGUI();
            //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            Animotor animotor = (Animotor)target;
            var linkedAnimotor = animotor as LinkedAnimotor;

            EditorGUILayout.BeginHorizontal();
            var aniTree = EditorGUILayout.ObjectField(animotor.aniTree, typeof(AniTree), false) as AniTree;
            if (aniTree != animotor.aniTree) {
                animotor.aniTree = aniTree;
                animotor.UpdateTree();
            }
            EditorGUI.BeginDisabledGroup(aniTree == null);
            if (GUILayout.Button("Open")) {
                AniTreeEditor.ShowWindow(aniTree);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (animotor.aniTree != null) {
                var centeredStyle = new GUIStyle("label");
                centeredStyle.alignment = TextAnchor.MiddleCenter;

                var centeredTitleStyle = new GUIStyle("label");
                centeredTitleStyle.alignment = TextAnchor.MiddleCenter;
                centeredTitleStyle.fontStyle = FontStyle.Bold;

                var clip = animotor.animotionClip;
                GUILayout.Label($"{(clip != null ? clip.name : "No clip")} - ({animotor.frame}) {" - " + animotor.direction}", centeredTitleStyle);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


                GUILayout.Label("Children", centeredTitleStyle);
                GUILayout.Label(animotor.currentNode.nodeName, centeredStyle);

                GUILayout.Label("Children", centeredTitleStyle);
                foreach (var child in animotor.currentNodeChildren) {
                    GUILayout.Label(child.nodeName, centeredStyle);
                }

                GUILayout.Label("Links", centeredTitleStyle);
                foreach (var link in animotor.currentNodeLinks) {
                    var node = aniTree.GetNode(link.endNodeId);
                    GUILayout.Label($"$-> {node.nodeName}", centeredStyle);
                }

                GUILayout.Label("Reverse Links", centeredTitleStyle);
                foreach (var reverseLink in animotor.currentNodeReverseLinks) {
                    var node = aniTree.GetNode(reverseLink.endNodeId);
                    GUILayout.Label($"{node.nodeName} ->", centeredStyle);
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                foreach (TreeProperty property in animotor.properties) {
                    string propertyName = property.name;
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label(propertyName);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(animotor.GetObject(propertyName).ToString());

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (linkedAnimotor != null) {
                var baseAnimotor = EditorGUILayout.ObjectField(linkedAnimotor.GetAnimotor(), typeof(Animotor), true) as Animotor;
                if (linkedAnimotor.GetAnimotor() != baseAnimotor) {
                    linkedAnimotor.SetAnimotor(baseAnimotor);
                }
            }

            var linkedAnimotors = animotor.GetLinkedAnimotors();
            if (linkedAnimotors.Any()) {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUI.BeginDisabledGroup(true);
                foreach (var linkedAnimotorContained in linkedAnimotors) {
                    EditorGUILayout.ObjectField(linkedAnimotorContained, typeof(LinkedAnimotor), true);
                }
                EditorGUI.EndDisabledGroup();
            }

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(animotor);
            }
        }

    }
}