using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animotion {
    public class Animotion : MonoBehaviour {

        public AnimotionClip animotionClip;
        private SpriteRenderer spriteRenderer;
        public bool animateOnStart = true;
        private int frame;
        private bool isTimerRunning;

        private void Start() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (animotionClip && animateOnStart) {
                spriteRenderer.sprite = animotionClip.GetFrame(0).sprite;
            }
            isTimerRunning = animateOnStart;
        }

        private void FixedUpdate() {
            if (isTimerRunning) {
                if (animotionClip) {
                    frame += 1;
                    FrameData frameData = animotionClip.GetLastFrame(frame);
                    if (frameData == null) Debug.Log(frame + " (" + frame + ")/" + animotionClip.length);
                    spriteRenderer.sprite = frameData.sprite;
                    if (frame >= animotionClip.length) {
                        frame = 0;
                        isTimerRunning = animotionClip.loop;
                    }
                }
                else {
                    frame = 0;
                }
            }
        }

        public void Play() {
            isTimerRunning = true;
        }

        public void Pause() {
            isTimerRunning = false;
        }
    }
}
