using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using UnityEditor;

namespace Animotion {
    [Serializable]
    public class AniNode : ScriptableObject {

        public static int idCounter;
        public int id;

        public bool isRoot;

        public Vector2 position;

        public string nodeName;

        public bool hasMultipleDirections;
        public AniClipGroup animotionClipsData;
        public AniClip animotionClip;

        public List<int> children;

        public bool waitForEnd = false;

        public void SetValues(string n, Vector2 _position) {
            id = idCounter;
            idCounter++;
            nodeName = n;
            position = _position;
            children = new List<int>();
        }

        public void SetValues(string n = "node") {
            SetValues(n, Vector2.zero);
        }

        public AniClip GetAnimotionClip() {
            return animotionClip;
        }

        public AniClip GetAnimotionClip(AniDirection direction) {
            return animotionClipsData.GetAnimotionClip(direction);
        }


        public int childrenCount {
            get {
                return children.Count;
            }
        }

    }
}
