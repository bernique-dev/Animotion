using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using UnityEditor;

namespace Animotion {
    [Serializable]
    public class AniNode : ScriptableObject {
        public enum NodeType {
            Default, Global
        }

        public static int idCounter;
        public int id;

        public bool isRoot;

        public Vector2 position;

        public string nodeName;

        public bool hasMultipleDirections;
        public AniClipGroup clipGroup;
        public AniClip clip;

        public List<int> children;

        public bool waitForEnd = false;

        public NodeType type;

        public void SetValues(string n, Vector2 _position, NodeType type = NodeType.Default) {
            id = idCounter;
            idCounter++;
            nodeName = n;
            position = _position;
            children = new List<int>();
            this.type = type;
        }

        public void SetValues(string n = "node") {
            SetValues(n, Vector2.zero);
        }

        public AniClip GetAnimotionClip() {
            return clip;
        }

        public AniClip GetAnimotionClip(AniDirection direction) {
            if (clipGroup == null) {
                return null;
            }
            return clipGroup.GetAnimotionClip(direction);
        }


        public int childrenCount {
            get {
                return children.Count;
            }
        }

    }
}
