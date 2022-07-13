using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

namespace Animotion {
    [CustomEditor(typeof(AnimotionClipsData))]
    public class AnimotionClipsDataEditor : Editor {

        private SerializedProperty downACProperty;
        private SerializedProperty leftACProperty;
        private SerializedProperty rightACProperty; 
        private SerializedProperty topACProperty;

        private SerializedProperty downLeftACProperty;
        private SerializedProperty downRightACProperty;
        private SerializedProperty topLeftACProperty;
        private SerializedProperty topRightACProperty;

        private AnimotionClipsDataMode mode {
            get {
                return animotionClips.mode;
            }
        }

        private AnimotionClipsData animotionClips {
            get {
                return target as AnimotionClipsData;
            }
        }

        void OnEnable() {
            downACProperty = serializedObject.FindProperty("downAnimotionClip");
            leftACProperty = serializedObject.FindProperty("leftAnimotionClip");
            rightACProperty = serializedObject.FindProperty("rightAnimotionClip");
            topACProperty = serializedObject.FindProperty("topAnimotionClip");

            downLeftACProperty = serializedObject.FindProperty("downLeftAnimotionClip");
            downRightACProperty = serializedObject.FindProperty("downRightAnimotionClip");
            topLeftACProperty = serializedObject.FindProperty("topLeftAnimotionClip");
            topRightACProperty = serializedObject.FindProperty("topRightAnimotionClip");
        }
        public override void OnInspectorGUI() {

            animotionClips.mode = (AnimotionClipsDataMode)EditorGUILayout.EnumPopup((AnimotionClipsDataMode)animotionClips.mode);

            DrawClipFields();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();


        }

        public void DrawClipFields() {
            Vector2 size = new Vector2(Screen.width* .31f, 50);
            float offset = 10;

            EditorGUILayout.BeginHorizontal(GUILayout.Width(size.x* 3));
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginVertical(GUILayout.Width(size.x), GUILayout.Height(size.y* 3));
            if (mode.Contains(AniDirection.UpLeft)) EditorGUILayout.PropertyField(topLeftACProperty, GUIContent.none, GUILayout.Height(size.y)); else GUILayout.FlexibleSpace();
            if (mode.Contains(AniDirection.Left)) EditorGUILayout.PropertyField(leftACProperty, GUIContent.none, GUILayout.Height(size.y)); else GUILayout.FlexibleSpace();
            if (mode.Contains(AniDirection.DownLeft)) EditorGUILayout.PropertyField(downLeftACProperty, GUIContent.none, GUILayout.Height(size.y)); else GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.Width(size.x), GUILayout.Height(size.y* 3 + 4 * offset / 10));
            if (mode.Contains(AniDirection.Up)) EditorGUILayout.PropertyField(topACProperty, GUIContent.none, GUILayout.Height(size.y)); else GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            if (mode.Contains(AniDirection.Down)) EditorGUILayout.PropertyField(downACProperty, GUIContent.none, GUILayout.Height(size.y)); else GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.Width(size.x), GUILayout.Height(size.y* 3));
            if (mode.Contains(AniDirection.DownRight)) EditorGUILayout.PropertyField(topRightACProperty, GUIContent.none, GUILayout.Height(size.y)); else GUILayout.FlexibleSpace();
            if (mode.Contains(AniDirection.Right)) EditorGUILayout.PropertyField(rightACProperty, GUIContent.none, GUILayout.Height(size.y)); else GUILayout.FlexibleSpace();
            if (mode.Contains(AniDirection.UpRight)) EditorGUILayout.PropertyField(downRightACProperty, GUIContent.none, GUILayout.Height(size.y)); else GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        
    }

    

}