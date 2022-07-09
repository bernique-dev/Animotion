using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Reset))]
public class ResetEditor : Editor {

    public override void OnInspectorGUI() {
        Reset reset = ((Reset)target);
        if (GUILayout.Button("Reset")) {
            reset.ResetScene();
        }
        reset.canResetWithKeyboard = EditorGUILayout.Toggle(reset);
    }
}
