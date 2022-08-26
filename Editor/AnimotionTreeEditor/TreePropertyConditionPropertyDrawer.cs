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
            TreeData tree = treeSerializedProperty != null ? (TreeData)treeSerializedProperty.objectReferenceValue : null;

            //Debug.Log(propertySerializedProperty);
            //Debug.Log(treeSerializedProperty);

            EditorGUI.BeginChangeCheck();

            if (tree) {
                int propertyIndex = EditorGUI.Popup(new Rect(position.min, new Vector2(position.width / 3, position.height)), treeProperty != null ? tree.properties.IndexOf(treeProperty) : 0, tree.properties.Select(p => p.name).ToArray());

                float fieldWidth = (2 * position.width / 3) / (tree.properties[propertyIndex].type.GetConditionFieldsNumber() - 1);
                //Debug.Log(property.FindPropertyRelative("treePropertyCondition").objectReferenceValue);
                switch (tree.properties[propertyIndex].type) {
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


                if (EditorGUI.EndChangeCheck()) {
                    property.FindPropertyRelative("property").objectReferenceValue = tree.properties[propertyIndex] as UnityEngine.Object;

                    switch (tree.properties[propertyIndex].type) {
                        case TreePropertyType.Boolean:
                            if (propertyConditionIndex >= TreePropertyCondition.GetBoolConditionMethods().Count) propertyConditionIndex = 0;
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
                }
            }

            //Handles.BeginGUI();
            //Handles.DrawSolidRectangleWithOutline(position, new Color32(0, 0, 0, 0), Color.yellow);
            //Handles.EndGUI();
        }

    }

}