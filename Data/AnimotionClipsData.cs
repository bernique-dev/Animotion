using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Animotion;

namespace Animotion {
    [Serializable]
    [CreateAssetMenu(menuName = "Animotion/Animotion Clips")]
    public class AnimotionClipsData : ScriptableObject {

        public AnimotionClipsDataMode mode;

        public AnimotionClip downAnimotionClip;
        public AnimotionClip leftAnimotionClip;
        public AnimotionClip rightAnimotionClip;
        public AnimotionClip upAnimotionClip;

        public AnimotionClip downLeftAnimotionClip;
        public AnimotionClip downRightAnimotionClip;
        public AnimotionClip upLeftAnimotionClip;
        public AnimotionClip upRightAnimotionClip;


        public AnimotionClip GetAnimotionClip(AniDirection direction) {
            if (mode.Contains(direction)) {
                AnimotionClip clip = null;
                switch (direction) {
                    case AniDirection.Down:
                        clip = downAnimotionClip;
                        break;
                    case AniDirection.DownLeft:
                        clip = downLeftAnimotionClip;
                        break;
                    case AniDirection.DownRight:
                        clip = downRightAnimotionClip;
                        break;
                    case AniDirection.Left:
                        clip = leftAnimotionClip;
                        break;
                    case AniDirection.Right:
                        clip = rightAnimotionClip;
                        break;
                    case AniDirection.Up:
                        clip = upAnimotionClip;
                        break;
                    case AniDirection.UpLeft:
                        clip = upLeftAnimotionClip;
                        break;
                    case AniDirection.UpRight:
                        clip = upRightAnimotionClip;
                        break;
                }

                return clip;
            } else {
                throw new Exception(" Unvalid AnimotionClipsDataMode | " + direction + " asked while mode is " + mode);
            }
        }
    }
}

