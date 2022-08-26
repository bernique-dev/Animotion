using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Animotion {
    [CustomEditor(typeof(AnimotionAnimator))]
    public class AnimotionAnimatorEditor : Editor {


        public override void OnInspectorGUI() {
            AnimotionAnimator animotionAnimator = (AnimotionAnimator)target;

            if (animotionAnimator.treeData != null) {
                foreach (TreeProperty property in animotionAnimator.treeData.properties) {
                    string propertyName = property.name;
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label(propertyName);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(animotionAnimator.GetObject(propertyName).ToString());

                    EditorGUILayout.EndHorizontal();
                }
            }

            base.OnInspectorGUI();
        }

    }
}