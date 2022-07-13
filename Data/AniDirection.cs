using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animotion {
    public enum AniDirection {
        Down, DownLeft, DownRight, Left, Right, Top, TopLeft, TopRight
    }

    public static class AniDirectionExtensions {

        public static Vector2 GetVector2(this AniDirection aniDirection) {
            Vector2 result = Vector2.zero;
            switch (aniDirection) {
                case AniDirection.Down:
                    result = Vector2.down;
                    break;
                case AniDirection.DownLeft:
                    result = (AniDirection.Down.GetVector2() + AniDirection.Left.GetVector2()).normalized;
                    break;
                case AniDirection.DownRight:
                    result = (AniDirection.Down.GetVector2() + AniDirection.Right.GetVector2()).normalized;
                    break;
                case AniDirection.Left:
                    result = Vector2.left;
                    break;
                case AniDirection.Right:
                    result = Vector2.right;
                    break;
                case AniDirection.Top:
                    result = Vector2.up;
                    break;
                case AniDirection.TopLeft:
                    result = (AniDirection.Top.GetVector2() + AniDirection.Left.GetVector2()).normalized;
                    break;
                case AniDirection.TopRight:
                    result = (AniDirection.Top.GetVector2() + AniDirection.Right.GetVector2()).normalized;
                    break;
            }
            return result;
        }

        public static AniDirection GetAniDirection(this Vector2 vector, AnimotionClipsDataMode mode = AnimotionClipsDataMode.EightDirections) {
            AniDirection aniDirection = AniDirection.Down;
            switch (mode) {
                case AnimotionClipsDataMode.TwoDirections:
                    aniDirection = vector.x > 0 ? AniDirection.Right : AniDirection.Left;
                    break;
                case AnimotionClipsDataMode.FourDirections:
                    if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y)) {
                        aniDirection = vector.x > 0 ? AniDirection.Right : AniDirection.Left;
                    } else {
                        aniDirection = vector.y > 0 ? AniDirection.Top : AniDirection.Down;
                    }
                    break;
                case AnimotionClipsDataMode.EightDirections:
                    if (vector.y > 0) {
                        if (vector.x > 0) {
                            aniDirection = AniDirection.TopRight;
                        }
                        else if (vector.x < 0) {
                            aniDirection = AniDirection.TopLeft;
                        }
                        else {
                            aniDirection = AniDirection.Top;
                        }
                    }
                    else if (vector.y < 0) {
                        if (vector.x > 0) {
                            aniDirection = AniDirection.DownRight;
                        }
                        else if (vector.x < 0) {
                            aniDirection = AniDirection.DownLeft;
                        }
                        else {
                            aniDirection = AniDirection.Down;
                        }
                    }
                    else {
                        if (vector.x > 0) {
                            aniDirection = AniDirection.Right;
                        }
                        else if (vector.x < 0) {
                            aniDirection = AniDirection.Left;
                        }
                    }
                    break;
            }
            

            return aniDirection;
        }

    }

}