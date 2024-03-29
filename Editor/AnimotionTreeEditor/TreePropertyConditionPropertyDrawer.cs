using System.Linq;
using System.Linq.Expressions;
using System;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    [CustomPropertyDrawer(typeof(TreePropertyCondition))]
    public class TreePropertyConditionPropertyDrawer : PropertyDrawer {

        private object value;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            int propertyConditionIndex = 0;

            //Debug.Log(property.type);

            SerializedProperty propertySerializedProperty = property.FindPropertyRelative(nameof(TreePropertyCondition.property));
            TreeProperty treeProperty = propertySerializedProperty != null ? (TreeProperty)propertySerializedProperty.objectReferenceValue : null;
            SerializedProperty treeSerializedProperty = property.FindPropertyRelative(nameof(TreePropertyCondition.tree));
            AniTree tree = treeSerializedProperty != null ? (AniTree)treeSerializedProperty.objectReferenceValue : null;

            //Debug.Log(propertySerializedProperty);
            //Debug.Log(treeSerializedProperty);

            //EditorGUI.BeginChangeCheck();

            if (tree) {
                if (tree.GetProperties().Any()) {
                    int propertyIndex = EditorGUI.Popup(new Rect(position.min, new Vector2(position.width / 3, position.height)), treeProperty != null ? tree.GetProperties().IndexOf(treeProperty) : 0, tree.GetProperties().Select(p => p.name).ToArray());

                    float fieldWidth = (2 * position.width / 3) / (tree.GetProperties()[propertyIndex].type.GetConditionFieldsNumber() - 1);
                    //Debug.Log(property.FindPropertyRelative("treePropertyCondition").objectReferenceValue);
                    switch (tree.GetProperties()[propertyIndex].type) {
                        case TreePropertyType.Boolean:
                            propertyConditionIndex = EditorGUI.Popup(new Rect(position.min + new Vector2(position.width - fieldWidth, 0), new Vector2(fieldWidth, position.height)), property.FindPropertyRelative("conditionIndex").intValue, TreePropertyCondition.GetBoolConditionMethods().Select(expr => expr.name).ToArray());
                            break;
                        case TreePropertyType.Trigger:
                            break;
                        case TreePropertyType.Integer:
                            propertyConditionIndex = EditorGUI.Popup(new Rect(position.min + new Vector2(fieldWidth, 0), new Vector2(fieldWidth * 1.5f, position.height)), property.FindPropertyRelative("conditionIndex").intValue, TreePropertyCondition.GetIntConditionMethods().Select(expr => expr.name).ToArray());
                            value = EditorGUI.IntField(new Rect(position.min + new Vector2(fieldWidth * 2.5f, 0), new Vector2(fieldWidth * 0.5f, position.height)), property.FindPropertyRelative("intValue").intValue);
                            break;
                        case TreePropertyType.Float:
                            propertyConditionIndex = EditorGUI.Popup(new Rect(position.min + new Vector2(fieldWidth, 0), new Vector2(fieldWidth * 1.5f, position.height)), property.FindPropertyRelative("conditionIndex").intValue, TreePropertyCondition.GetFloatConditionMethods().Select(expr => expr.name).ToArray());
                            value = EditorGUI.FloatField(new Rect(position.min + new Vector2(fieldWidth * 2.5f, 0), new Vector2(fieldWidth * 0.5f, position.height)), property.FindPropertyRelative("floatValue").floatValue);
                            break;
                    }


                    //if (EditorGUI.EndChangeCheck()) {
                    property.FindPropertyRelative("property").objectReferenceValue = tree.GetProperties()[propertyIndex] as UnityEngine.Object;

                    switch (tree.GetProperties()[propertyIndex].type) {
                        case TreePropertyType.Boolean:
                            if (propertyConditionIndex >= TreePropertyCondition.GetBoolConditionMethods().Count)
                                propertyConditionIndex = 0;
                            break;
                        case TreePropertyType.Trigger:
                            break;
                        case TreePropertyType.Integer:
                            property.FindPropertyRelative("intValue").intValue = (int)value;
                            break;
                        case TreePropertyType.Float:
                            property.FindPropertyRelative("floatValue").floatValue = (float)value;
                            break;
                    }
                    property.FindPropertyRelative("conditionIndex").intValue = propertyConditionIndex;
                    //}
                } else {
                    GUI.Label(position, "No property");
                }
            }

            //Handles.BeginGUI();
            //Handles.DrawSolidRectangleWithOutline(position, new Color32(0, 0, 0, 0), Color.yellow);
            //Handles.EndGUI();
        }

    }

}