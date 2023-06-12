using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class AniClipEditorWindowPreview : AnimotionEditorWindowComponent {

        /// <summary>
        /// EditorWindow being used
        /// </summary>
        public AniClipEditorWindow animotionEditorWindow;

        public Texture texture;

        public Vector2 size {
            get {
                return _size != null ? _size : SIZEREFERENCE;
            }
        }
        private Vector2 _size;
        private readonly Vector2 SIZEREFERENCE = new Vector2(200, 200);

        public Vector2 center;

        public override void Draw() {
            center = new Vector2(Screen.width / 2, Screen.height / 3);

            if (animotionEditorWindow.animotionClip) {
                int framesPassed = (int)animotionEditorWindow.framesPassed;
                AniFrame frameData = animotionEditorWindow.animotionClip.GetLastFrame(framesPassed);
                if (frameData != null) {
                    Sprite sprite = frameData.sprite;
                    if (sprite) {
                        _size = new Vector2(SIZEREFERENCE.x * sprite.rect.width / sprite.rect.height, SIZEREFERENCE.y);
                        Rect rect = RectUtils.GetRect(center, size);
                        EditorGUI.DrawRect(rect, new Color32(20, 20, 20, 50));
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
