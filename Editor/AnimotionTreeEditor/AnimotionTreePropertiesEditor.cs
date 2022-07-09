using System.Collections;
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

        public override void Draw() {
            base.Draw();
            Handles.BeginGUI();

            Handles.color = AnimotionTreeEditor.BORDER_COLOR;
            Handles.DrawLine(new Vector2(rect.xMax, rect.yMin), rect.max);
            Handles.color = Color.white;

            if (animotionTreeEditor.tree) {
                Texture2D buttonTexture = EditorGUIUtility.Load("Assets/Animotion/Editor/Sprites/medical_healthcare_cross.png") as Texture2D;

                if (GUI.Button(RectUtils.GetRect(new Vector2(rect.width / 2, 35), new Vector2(25, 25)), buttonTexture)) {

                }

                Handles.DrawLine(new Vector2(rect.xMin, 60), new Vector2(rect.xMax, 60));
                for (int i = 0; i < animotionTreeEditor.tree.booleanList.Count; i++) {
                    string booleanName = animotionTreeEditor.tree.booleanList[i];

                    Rect propertyRect = new Rect(new Vector2(rect.xMin, 60 + 25 * i), new Vector2(rect.width, 25));

                    bool isBoolTrue = false;
                    if (animotionTreeEditor.animotionAnimator) {
                        isBoolTrue = animotionTreeEditor.animotionAnimator.GetBool(booleanName);
                    }

                    Handles.DrawSolidRectangleWithOutline(propertyRect, isBoolTrue ? AnimotionTreeEditor.BACKGROUND_COLOR : AnimotionTreeEditor.LIGHT_BACKGROUND_COLOR, AnimotionTreeEditor.BORDER_COLOR);
                    Handles.Label(new Vector2(20, 65 + i * 25), booleanName);
                }
            }

            Handles.EndGUI();
        }

        public override void ProcessEvent(Event e) {
            base.ProcessEvent(e);
        }
    }

}