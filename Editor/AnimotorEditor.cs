﻿using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Animotion {
    [CustomEditor(typeof(Animotor))]
    public class AnimotorEditor : Editor {

        public override void OnInspectorGUI() {
            Animotor animotor = (Animotor)target;

            animotor.aniTree = (AniTree)EditorGUILayout.ObjectField(animotor.aniTree, typeof(AniTree), false);

            if (animotor.aniTree != null) {
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