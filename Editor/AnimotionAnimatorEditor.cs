using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Animotion {
    [CustomEditor(typeof(AnimotionAnimator))]
    public class AnimotionAnimatorEditor : Editor {


        public override void OnInspectorGUI() {
            AnimotionAnimator animotionAnimator = (AnimotionAnimator)target;

            if (animotionAnimator.treeData != null) {
                foreach (string boolName in animotionAnimator.treeData.booleanList) {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label(boolName);
                    GUILayout.FlexibleSpace();

                    bool currentBool = animotionAnimator.GetBool(boolName);
                    bool modifiedBool = EditorGUILayout.Toggle(currentBool);
                    if (currentBool != modifiedBool) {
                        animotionAnimator.SetBool(boolName, modifiedBool);
                    }


                    EditorGUILayout.EndHorizontal();
                }
            }

            base.OnInspectorGUI();
        }

    }
}