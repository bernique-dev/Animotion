using UnityEditor;
using UnityEngine;

namespace Animotion {
    [CustomEditor(typeof(AnimotionPlayer), true)]
    public class AnimotionPlayerEditor : Editor
    {
        public override void OnInspectorGUI() {
            var player = (AnimotionPlayer)target;
            
            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button(player.isPlaying ? "Pause" : "Play")) {
                if (player.isPlaying) {
                    player.Pause();
                } else {
                    player.Play();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
