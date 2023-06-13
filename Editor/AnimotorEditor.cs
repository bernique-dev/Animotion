using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Animotion {
    [CustomEditor(typeof(Animotor))]
    public class AnimotorEditor : Editor {

        public override void OnInspectorGUI() {

            EditorGUI.BeginChangeCheck();
            //base.OnInspectorGUI();
            //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            Animotor animotor = (Animotor)target;

            var aniTree = EditorGUILayout.ObjectField(animotor.aniTree, typeof(AniTree), false) as AniTree;
            if (aniTree != animotor.aniTree) {
                Debug.Log(aniTree);
                animotor.aniTree = aniTree;
                animotor.UpdateTree();
            }
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
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(animotor);
            }
        }

    }
}