using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Animotion;

namespace Animotion {
    [Serializable]
    [CreateAssetMenu(menuName = "Animotion/Animotion Clips")]
    public class AniClipGroup : ScriptableObject {

        public AniClipGroupMode mode;

        public AniClip downAnimotionClip;
        public AniClip leftAnimotionClip;
        public AniClip rightAnimotionClip;
        public AniClip upAnimotionClip;

        public AniClip downLeftAnimotionClip;
        public AniClip downRightAnimotionClip;
        public AniClip upLeftAnimotionClip;
        public AniClip upRightAnimotionClip;


        public AniClip GetAnimotionClip(AniDirection direction) {
            if (mode.Contains(direction)) {
                AniClip clip = null;
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
                throw new Exception("Unvalid AnimotionClipsDataMode | " + direction + " asked while mode is " + mode);
            }
        }
    }
}

