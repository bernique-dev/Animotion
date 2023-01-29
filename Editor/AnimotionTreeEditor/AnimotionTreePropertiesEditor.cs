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


        private List<TreeProperty> properties {
            get {
                List<TreeProperty> tmpProperties = animotionTreeEditor.tree.properties;
                if (animotionTreeEditor.animotionAnimator) {
                    if (Application.isPlaying) {
                        tmpProperties = animotionTreeEditor.animotionAnimator.properties;
                    }
                }

                return tmpProperties;
            }
        }

        public override void Draw() {
            base.Draw();
            Handles.BeginGUI();

            Handles.color = AnimotionTreeEditor.BORDER_COLOR;
            Handles.DrawLine(new Vector2(rect.xMax, rect.yMin), rect.max);
            Handles.color = Color.white;

            if (animotionTreeEditor.tree) {

                if (Application.isPlaying) {
                    EditorGUI.BeginDisabledGroup(true);
                }
                Texture2D buttonTexture = EditorGUIUtility.Load("Assets/Animotion/Editor/Sprites/medical_healthcare_cross.png") as Texture2D;

                Rect newPropertyFields = new Rect(new Vector2(5, rect.yMin + 5 * 9 / 16), new Vector2(rect.width - 5, 25));
                if (GUI.Button(new Rect(newPropertyFields.min, new Vector2(newPropertyFields.height, newPropertyFields.height)), buttonTexture)) {
                    TreeProperty property = CreateInstance<TreeProperty>();
                    property.SetValues(newPropertyType.ToString(), newPropertyType);
                    switch (property.type) {
                        case TreePropertyType.Boolean:
                            property.value = false;
                            break;
                        case TreePropertyType.Trigger:
                            property.value = false;
                            break;
                        case TreePropertyType.Integer:
                            break;
                        case TreePropertyType.Float:
                            break;
                    }

                    animotionTreeEditor.tree.AddProperty(property);
                    EditorUtility.SetDirty(animotionTreeEditor.tree);
                }
                GUIStyle popupStyle = new GUIStyle(EditorStyles.popup);
                popupStyle.fixedHeight = newPropertyFields.height;
                newPropertyType = (TreePropertyType)EditorGUI.EnumPopup(new Rect(newPropertyFields.min + new Vector2(30,0), new Vector2(newPropertyFields.width - 35, newPropertyFields.height)) , newPropertyType, popupStyle);


                if (Application.isPlaying) {
                    EditorGUI.EndDisabledGroup();
                }

                if (animotionTreeEditor.animotionAnimator) {
                } else {
                    if (Application.isPlaying) {
                        EditorGUI.BeginDisabledGroup(true);
                    }
                }

                Handles.DrawLine(new Vector2(rect.xMin, 60), new Vector2(rect.xMax, 60));
                for (int i = 0; i < properties.Count; i++) {
                    TreeProperty property = properties[i];

                    Rect propertyRect = new Rect(new Vector2(rect.xMin, 60 + 25 * i), new Vector2(rect.width, 25));

                    if (propertyRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick) {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Delete"), false, () => animotionTreeEditor.tree.DeleteProperty(property));
                        menu.ShowAsContext();
                    }


                    Handles.DrawSolidRectangleWithOutline(propertyRect, AnimotionTreeEditor.LIGHT_BACKGROUND_COLOR, AnimotionTreeEditor.BORDER_COLOR);
                    //Handles.Label(new Vector2(20, 65 + i * 25), booleanName);

                    Rect propertyFieldRect = new Rect(new Vector2(rect.xMin + 2.5f, 60 + 25 * i + 2f), new Vector2(rect.width - 5f, 20));
                    float gap = 5f;
                    Rect propertyNameFieldRect = new Rect(propertyFieldRect.min, new Vector2(4 * propertyFieldRect.width / 5 - gap / 2, propertyFieldRect.height));
                    property.name = EditorGUI.TextField(propertyNameFieldRect, property.name);

                    EditorGUI.BeginChangeCheck();

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
                            property.value = EditorGUI.IntField(new Rect(new Vector2(propertyFieldRect.xMax - propertyFieldRect.height * 1.45f, propertyFieldRect.yMin), new Vector2(propertyFieldRect.height* 1.45f, propertyFieldRect.height)), property.value == null ? 0 : (int)property.value);
                            break;
                        case TreePropertyType.Float:
                            property.value = EditorGUI.FloatField(new Rect(new Vector2(propertyFieldRect.xMax - propertyFieldRect.height * 1.45f, propertyFieldRect.yMin), new Vector2(propertyFieldRect.height * 1.45f, propertyFieldRect.height)), property.value == null ? 0 : (float)property.value);
                            break;
                    }

                    if (EditorGUI.EndChangeCheck()) {
                        //if (animotionTreeEditor.animotionAnimator) {
                        //    animotionTreeEditor.animotionAnimator.UpdateProperties();
                        //}
                    }

                    EditorUtility.SetDirty(property);
                }

                if (animotionTreeEditor.animotionAnimator) {
                } else {
                    if (Application.isPlaying) {
                        EditorGUI.EndDisabledGroup();
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