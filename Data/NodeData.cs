using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;

namespace Animotion {
    [Serializable]
    public class NodeData : ScriptableObject {

        public static int idCounter;
        public int id;

        public bool isRoot;

        public Vector2 position;

        public string nodeName;

        /// <summary>
        /// Only used for serialization
        /// </summary>
        [HideInInspector] public string animotionClipsDataPath;
        [HideInInspector] public string animotionClipPath;

        public bool hasMultipleDirections;
        public AnimotionClipsData animotionClipsData;
        public AnimotionClip animotionClip;

        public List<int> children;

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

        public AnimotionClip GetAnimotionClip() {
            return animotionClip;
        }

        public AnimotionClip GetAnimotionClip(AniDirection direction) {
            return animotionClipsData.GetAnimotionClip(direction);
        }


        public int childrenCount {
            get {
                return children.Count;
            }
        }

        public void SaveData() {
            if (animotionClipsData) {
                animotionClipsDataPath = AssetDatabase.GetAssetPath(animotionClipsData);
            }
            if (animotionClip) {
                animotionClipPath = AssetDatabase.GetAssetPath(animotionClip);
            }
        }


    }
}
