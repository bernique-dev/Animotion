using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    [CustomEditor(typeof(NodeData))]
    public class NodeDataEditor : Editor {

        private AnimotionType animotionType;

        public override void OnInspectorGUI() {
            //base.OnInspectorGUI();
            NodeData node = target as NodeData;

            node.nodeName = EditorGUILayout.TextField("Name", node.nodeName);

            animotionType = (AnimotionType)EditorGUILayout.EnumPopup("Type", node.hasMultipleDirections ? AnimotionType.WithDirections : AnimotionType.WithoutDirections);
            node.hasMultipleDirections = animotionType == AnimotionType.WithDirections;

            if (node.hasMultipleDirections) {
                node.animotionClipsData = (AnimotionClipsData)EditorGUILayout.ObjectField("Animotions", node.animotionClipsData, typeof(AnimotionClipsData), false);
            } else {
                node.animotionClip = (AnimotionClip)EditorGUILayout.ObjectField("Animotion", node.animotionClip, typeof(AnimotionClip), false);
            }

            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            int sideGap = 35;
            int upGap = 4;
            Handles.DrawLine(new Vector2(rect.x + sideGap, rect.y + upGap), new Vector2(rect.width - sideGap, rect.y + upGap));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            node.waitForEnd = EditorGUILayout.Toggle("Wait for end", node.waitForEnd);

        }

        private enum AnimotionType {
            WithDirections, WithoutDirections
        }
    }
}