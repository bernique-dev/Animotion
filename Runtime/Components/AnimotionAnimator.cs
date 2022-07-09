using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class AnimotionAnimator : MonoBehaviour {

        private SpriteRenderer spriteRenderer;


        public NodeData currentNode {
            get {
                return m_currentNode;
            }
             set {
                m_currentNode = value;
                if (value) {
                    currentNodeChildren = currentNode.children.Select(id => treeData.nodes.Find(t => t.id == id)).ToList();
                    currentNodeLinks = currentNode.children.Select(id => treeData.links.Find(l => (l.startNodeId == currentNode.id && l.endNodeId == id) || (l.endNodeId == currentNode.id && l.startNodeId == id && l is BidirectionalLinkData))).Distinct().ToList();
                }
            }
        }
        public NodeData m_currentNode;

        public List<NodeData> currentNodeChildren;
        public List<LinkData> currentNodeLinks;
        public List<BidirectionalLinkData> currentNodeReverseLinks;

        public TreeData treeData;
        // Hideable
        public AniDirection aniDirection;
        public AnimotionClip animotionClip {
            get {
                return currentNode ? currentNode.hasMultipleDirections ? currentNode.GetAnimotionClip(aniDirection):currentNode.GetAnimotionClip(): null;
            }
        }

        public bool animateOnStart = true;

        public Dictionary<string, bool> booleans;


        public int frame;
        private bool isTimerRunning;

        private void Start() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (animotionClip) {
                spriteRenderer.sprite = animotionClip.GetFrame(0).sprite;
            }
            isTimerRunning = animateOnStart;
            currentNode = treeData.root;
        }

        private void InitiateBooleans() {
            booleans = new Dictionary<string, bool>();
            foreach (string boolName in treeData.booleanList) {
                booleans.Add(boolName, false);
            }
        }

        public void SetDirection(Vector2 delta) {
            SetDirection(delta.GetAniDirection());
        }

        public void SetDirection(AniDirection _aniDirection) {
            aniDirection = _aniDirection;
        } 

        public void SetBool(string boolName, bool value) {
            if (booleans == null) InitiateBooleans();
            booleans[boolName] = value;
        }

        public bool GetBool(string boolName) {
            if (booleans == null) InitiateBooleans();
            return booleans[boolName];
        }

        private void Update() {

            if (Input.GetKeyDown(KeyCode.E)) {
                SetBool("isWalking", !GetBool("isWalking"));
            }
            if (Input.GetKeyDown(KeyCode.F)) {
                SetBool("isSwimming", !GetBool("isSwimming"));
            }
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
                } else {
                    frame = 0;
                }
            }

            foreach (LinkData link in currentNodeLinks) {
                if (link) {
                    bool takeLink = true;
                    foreach (string trueBoolName in link.trueBooleanNames) {
                        if (!GetBool(trueBoolName)) {
                            takeLink = false;
                            break;
                        }
                    }
                    foreach (string falseBoolName in link.falseBooleanNames) {
                        if (GetBool(falseBoolName)) {
                            takeLink = false;
                            break;
                        }
                    }
                    if (takeLink) {
                        currentNode = treeData.GetNode(link.endNodeId);
                    }

                    BidirectionalLinkData bidirectionalLink = link as BidirectionalLinkData;
                    if (bidirectionalLink) {
                        takeLink = true;
                        foreach (string trueBoolName in bidirectionalLink.reverseTrueBooleanNames) {
                            if (!GetBool(trueBoolName)) {
                                takeLink = false;
                                break;
                            }
                        }
                        foreach (string falseBoolName in bidirectionalLink.reverseFalseBooleanNames) {
                            if (GetBool(falseBoolName)) {
                                takeLink = false;
                                break;
                            }
                        }
                        if (takeLink) {
                            currentNode = treeData.GetNode(bidirectionalLink.startNodeId);
                            break;
                        }
                    }
                }
                
            }
        }

        private void OnDrawGizmos() {
#if UNITY_EDITOR
            if (currentNode) Handles.Label(transform.position,currentNode.nodeName);
#endif
        }

    }
}
