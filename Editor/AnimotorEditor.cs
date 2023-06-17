using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Animotion {
    [CustomEditor(typeof(Animotor), true)]
    public class AnimotorEditor : Editor {

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
                var style = new GUIStyle("label");
                style.alignment = TextAnchor.MiddleCenter;
                var clip = animotor.animotionClip;
                GUILayout.Label($"{clip.name} - ({animotor.frame}) {" - " + animotor.direction}", style);

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