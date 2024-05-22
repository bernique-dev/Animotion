using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Animotion {
    public class AnimotionPlayer : MonoBehaviour {

        public AniClip animotionClip;
        private SpriteRenderer spriteRenderer;
        private Image image;
        public bool animateOnStart = true;
        private int frame;
        public bool isPlaying {  get; private set; }

        protected void Start() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            if (animotionClip && animateOnStart) {
                spriteRenderer.sprite = animotionClip.GetFrame(0).sprite;
            }
            isPlaying = animateOnStart;
        }

        private void FixedUpdate() {
            if (isPlaying) {
                if (animotionClip) {
                    frame += 1;
                    AniFrame frameData = animotionClip.GetLastFrame(frame);
                    if (frameData == null) Debug.Log(frame + " (" + frame + ")/" + animotionClip.length);
                    if (spriteRenderer) spriteRenderer.sprite = frameData.sprite;
                    if (image) image.sprite = frameData.sprite;
                    if (frame >= animotionClip.length) {
                        frame = 0;
                        isPlaying = animotionClip.loop;
                    }
                }
                else {
                    frame = 0;
                }
            }
        }

        public void Play() {
            isPlaying = true;
        }

        public void Pause() {
            isPlaying = false;
        }
    }
}
