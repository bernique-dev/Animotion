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
        public AnimotionClip topAnimotionClip;

        public AnimotionClip downLeftAnimotionClip;
        public AnimotionClip downRightAnimotionClip;
        public AnimotionClip topLeftAnimotionClip;
        public AnimotionClip topRightAnimotionClip;


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
                    case AniDirection.Top:
                        clip = topAnimotionClip;
                        break;
                    case AniDirection.TopLeft:
                        clip = topLeftAnimotionClip;
                        break;
                    case AniDirection.TopRight:
                        clip = topRightAnimotionClip;
                        break;
                }

                return clip;
            } else {
                throw new Exception(" Unvalid AnimotionClipsDataMode | " + direction + " asked while mode is " + mode);
            }
        }
    }
}

