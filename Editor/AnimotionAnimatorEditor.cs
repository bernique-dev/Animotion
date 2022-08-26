using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Animotion {
    [CustomEditor(typeof(AnimotionAnimator))]
    public class AnimotionAnimatorEditor : Editor {


        public override void OnInspectorGUI() {
            AnimotionAnimator animotionAnimator = (AnimotionAnimator)target;

            if (animotionAnimator.treeData != null) {
                foreach (TreeProperty property in animotionAnimator.treeData.propertyList) {
                    string propertyName = property.name;
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label(propertyName);
                    GUILayout.FlexibleSpace();

                    bool currentBool = animotionAnimator.GetBool(propertyName);
                    bool modifiedBool = EditorGUILayout.Toggle(currentBool);
                    if (currentBool != modifiedBool) {
                        animotionAnimator.SetBool(propertyName, modifiedBool);
                    }


                    EditorGUILayout.EndHorizontal();
                }
            }

            base.OnInspectorGUI();
        }

    }
}