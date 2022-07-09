using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Animotion {

    public enum AnimotionClipsDataMode {
        TwoDirections, FourDirections, EightDirections
    }

    public static class AnimotionClipsDataModeExtensions {

        public static bool Contains(this AnimotionClipsDataMode mode, AniDirection direction) {
            bool result = false;
            switch (mode) {
                case AnimotionClipsDataMode.TwoDirections:
                    result = (new AniDirection[] { AniDirection.Left, AniDirection.Right }).Contains(direction);
                    break;
                case AnimotionClipsDataMode.FourDirections:
                    result = (new AniDirection[] { AniDirection.Down, AniDirection.Left, AniDirection.Right, AniDirection.Top }).Contains(direction);
                    break;
                case AnimotionClipsDataMode.EightDirections:
                    result = (new AniDirection[] { AniDirection.Down, AniDirection.DownLeft, AniDirection.DownRight, AniDirection.Left, AniDirection.Right, AniDirection.Top, AniDirection.TopLeft, AniDirection.TopRight }).Contains(direction);
                    break;
            }
            return result;
        }

    }

}