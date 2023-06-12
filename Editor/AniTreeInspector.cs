using UnityEditor;
using UnityEngine;
using Animotion;
using System.Collections.Generic;

[CustomEditor(typeof(AniTree), true)]
public class AniTreeInspector : Editor {


    public override void OnInspectorGUI() {
        var aniTree = target as AniTree;
        if (aniTree) {

            var variant = aniTree as AniTreeVariant;

            var style = new GUIStyle("label");
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(aniTree.name, style);

            if (variant) {
                var newTree = EditorGUILayout.ObjectField(variant.aniTree, typeof(AniTree), false) as AniTree;
                if (newTree as AniTreeVariant) {
                    Debug.LogError("Can't add Variant to a Variant", this);
                } else {
                    variant.aniTree = newTree;
                }
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            foreach (TreeProperty property in aniTree.GetProperties()) {
                string propertyName = property.name;
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(propertyName);
                GUILayout.FlexibleSpace();
                GUILayout.Label(property.value.ToString());

                EditorGUILayout.EndHorizontal();
            }

            if (variant) {
                if (variant.aniTree) {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    foreach (var binding in variant.bindings) {
                        var boundNode = variant.aniTree.GetNode(binding.nodeId);

                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label(boundNode.name);
                        GUILayout.FlexibleSpace();
                        if (binding.hasMultipleDirections) {
                            binding.clipGroup = EditorGUILayout.ObjectField(binding.clipGroup, typeof(AniClipGroup), false) as AniClipGroup;
                        }
                        else {
                            binding.clip = EditorGUILayout.ObjectField(binding.clip, typeof(AniClip), false) as AniClip;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                } else {
                    GUILayout.Label("No AniTree in variant", style);
                }
            }
        }
        //base.OnInspectorGUI();
    }
}