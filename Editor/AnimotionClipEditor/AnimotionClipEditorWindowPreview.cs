using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class AnimotionClipEditorWindowPreview : AnimotionEditorWindowComponent {

        /// <summary>
        /// EditorWindow being used
        /// </summary>
        public AnimotionClipEditorWindow animotionEditorWindow;

        public Texture texture;

        public Vector2 size {
            get {
                return new Vector2(200, 200);
            }
        }

        public Vector2 center;

        public override void Draw() {
            center = new Vector2(Screen.width / 2, Screen.height / 3);

            if (animotionEditorWindow.animotionClip) {
                Rect rect = RectUtils.GetRect(center, size);
                EditorGUI.DrawRect(rect, new Color32(20, 20, 20, 50));
                int framesPassed = (int)animotionEditorWindow.framesPassed;
                FrameData frameData = animotionEditorWindow.animotionClip.GetLastFrame(framesPassed);
                if (frameData != null) {
                    Sprite sprite = frameData.sprite;
                    if (sprite) {
                        texture = sprite.GetTexture();
                        GUI.DrawTexture(rect, texture);
                    }
                }
            }
        }

        public override void ProcessEvent(Event e) {
            base.ProcessEvent(e);


        }

    }

}
