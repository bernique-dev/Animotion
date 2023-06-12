using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Animotion {
    [CustomEditor(typeof(Animotor))]
    public class AnimotorEditor : Editor {

        public override void OnInspectorGUI() {
            //base.OnInspectorGUI();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            Animotor animotor = (Animotor)target;

            var aniTree = (AniTree)EditorGUILayout.ObjectField(animotor.aniTree, typeof(AniTree), false);
            if (aniTree != animotor.aniTree) {
                animotor.aniTree = aniTree;
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (animotor.aniTree != null) {
                var style = new GUIStyle("label");
                style.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label($"{animotor.animotionClip.name} - ({animotor.frame}) {" - " + animotor.direction}", style);

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
        }

    }
}