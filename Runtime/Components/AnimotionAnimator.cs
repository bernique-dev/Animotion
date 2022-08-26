using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class AnimotionAnimator : MonoBehaviour {

        private SpriteRenderer spriteRenderer;
        public bool animateOnStart = true;
        public int frame;
        private bool isTimerRunning;


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
        private AniDirection aniDirection = AniDirection.Right;
        public AnimotionClip animotionClip {
            get {
                return currentNode ? currentNode.hasMultipleDirections ? currentNode.GetAnimotionClip(aniDirection):currentNode.GetAnimotionClip(): null;
            }
        }


        public Dictionary<string, bool> booleans;



        private void Start() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (animotionClip) {
                spriteRenderer.sprite = animotionClip.GetFrame(0).sprite;
            }
            isTimerRunning = animateOnStart;
            if (treeData) currentNode = treeData.root;
        }

        private void InitiateBooleans() {
            booleans = new Dictionary<string, bool>();
            foreach (TreeProperty property in treeData.propertyList) {
                booleans.Add(property.name, false);
            }
        }

        public void SetDirection(Vector2 delta) {
            if (currentNode) {
                if (currentNode.hasMultipleDirections) {
                    SetDirection(delta.GetAniDirection(currentNode.animotionClipsData.mode));
                }
                else {
                    SetDirection(delta.GetAniDirection());
                }
            }
        }

        public void SetDirection(AniDirection _aniDirection) {
            if (currentNode.hasMultipleDirections) {
                aniDirection = _aniDirection.GetVector2().GetAniDirection(currentNode.animotionClipsData.mode);
            } else {
                aniDirection = _aniDirection;
            }
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

            //if (Input.GetKeyDown(KeyCode.E)) {
            //    SetBool("isWalking", !GetBool("isWalking"));
            //}
            //if (Input.GetKeyDown(KeyCode.F)) {
            //    SetBool("isSwimming", !GetBool("isSwimming"));
            //}
        }

        public void Play() {
            isTimerRunning = true;
        }

        public void Pause() {
            isTimerRunning = false;
        }

        private void FixedUpdate() {
            //Debug.LogError("FixedUpdate " + (isTimerRunning ? "[RUNNING]" : "[PAUSE]") + " " + (animateOnStart ? "[START]" : "[PAS START]") + " " + (animotionClip ? "[CLIP]" : "[PAS CLIP]"));
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
            if (!currentNode.waitForEnd || (frame == 0)) {
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
        }

        private void OnDrawGizmos() {
#if UNITY_EDITOR
            if (currentNode) Handles.Label(transform.position,currentNode.nodeName);
#endif
        }

    }
}
