using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Animotion {
    public class AnimotionTreePropertiesEditor : AnimotionEditorWindowComponent {

        public AnimotionTreeEditor animotionTreeEditor;

        public Rect rect {
            get {
                return new Rect(new Vector2(0, EditorStyles.toolbarButton.fixedHeight + 1), new Vector2(150, Screen.height - (EditorStyles.toolbarButton.fixedHeight + 1)));
            }
        }

        private TreePropertyType newPropertyType;

        public override void Draw() {
            base.Draw();
            Handles.BeginGUI();

            Handles.color = AnimotionTreeEditor.BORDER_COLOR;
            Handles.DrawLine(new Vector2(rect.xMax, rect.yMin), rect.max);
            Handles.color = Color.white;

            if (animotionTreeEditor.tree) {
                Texture2D buttonTexture = EditorGUIUtility.Load("Assets/Animotion/Editor/Sprites/medical_healthcare_cross.png") as Texture2D;

                Rect newPropertyFields = new Rect(new Vector2(5, rect.yMin + 5 * 9 / 16), new Vector2(rect.width - 5, 25));
                if (GUI.Button(new Rect(newPropertyFields.min, new Vector2(newPropertyFields.height, newPropertyFields.height)), buttonTexture)) {
                    TreeProperty property = new TreeProperty("property", newPropertyType);

                    switch (property.type) {
                        case TreePropertyType.Boolean:
                            property.value = false;
                            break;
                        case TreePropertyType.Trigger:
                            break;
                        case TreePropertyType.Integer:
                            break;
                        case TreePropertyType.Float:
                            break;
                    }

                    animotionTreeEditor.tree.propertyList.Add(property);
                }
                GUIStyle popupStyle = new GUIStyle(EditorStyles.popup);
                popupStyle.fixedHeight = newPropertyFields.height;
                newPropertyType = (TreePropertyType)EditorGUI.EnumPopup(new Rect(newPropertyFields.min + new Vector2(30,0), new Vector2(newPropertyFields.width - 35, newPropertyFields.height)) , newPropertyType, popupStyle);

                Handles.DrawLine(new Vector2(rect.xMin, 60), new Vector2(rect.xMax, 60));
                for (int i = 0; i < animotionTreeEditor.tree.propertyList.Count; i++) {
                    TreeProperty property = animotionTreeEditor.tree.propertyList[i];

                    Rect propertyRect = new Rect(new Vector2(rect.xMin, 60 + 25 * i), new Vector2(rect.width, 25));

                    bool isBoolTrue = false;
                    if (animotionTreeEditor.animotionAnimator) {
                        //isBoolTrue = animotionTreeEditor.animotionAnimator.GetBool(property);
                    }

                    Handles.DrawSolidRectangleWithOutline(propertyRect, isBoolTrue ? AnimotionTreeEditor.BACKGROUND_COLOR : AnimotionTreeEditor.LIGHT_BACKGROUND_COLOR, AnimotionTreeEditor.BORDER_COLOR);
                    //Handles.Label(new Vector2(20, 65 + i * 25), booleanName);

                    Rect propertyFieldRect = new Rect(new Vector2(rect.xMin + 2.5f, 60 + 25 * i + 2f), new Vector2(rect.width - 5f, 20));
                    float gap = 5f;
                    Rect propertyNameFieldRect = new Rect(propertyFieldRect.min, new Vector2(4 * propertyFieldRect.width / 5 - gap / 2, propertyFieldRect.height));
                    property.name = EditorGUI.TextField(propertyNameFieldRect, property.name);

                    switch (property.type) {
                        case TreePropertyType.Boolean:
                            property.value = EditorGUI.Toggle(new Rect(new Vector2(propertyFieldRect.xMax - propertyFieldRect.height, propertyFieldRect.yMin), new Vector2(propertyFieldRect.height, propertyFieldRect.height)), property.value == null ? false : (bool)property.value);
                            break;
                        case TreePropertyType.Trigger:
                            if (GUI.Button(new Rect(new Vector2(propertyFieldRect.xMax - propertyFieldRect.height * 1.15f, propertyFieldRect.yMin), new Vector2(propertyFieldRect.height, propertyFieldRect.height)), "")) {
                                property.value = true;
                            }
                            break;
                        case TreePropertyType.Integer:
                            break;
                        case TreePropertyType.Float:
                            break;
                    }

                }
            }
            Handles.EndGUI();
        }

        public override void ProcessEvent(Event e) {
            base.ProcessEvent(e);
        }
    }

}