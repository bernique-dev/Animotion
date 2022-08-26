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
        [SerializeField] private AniDirection aniDirection = AniDirection.Right;
        public AnimotionClip animotionClip {
            get {
                return currentNode ? currentNode.hasMultipleDirections ? currentNode.GetAnimotionClip(aniDirection):currentNode.GetAnimotionClip(): null;
            }
        }


        public List<TreeProperty> properties {
            get {
                return treeData ? treeData.properties : null;
            }
        }



        private void Start() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (animotionClip) {
                spriteRenderer.sprite = animotionClip.GetFrame(0).sprite;
            }
            isTimerRunning = animateOnStart;
            if (treeData) currentNode = treeData.root;
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

        public void SetObject(string propertyName, object value) {
            properties.Find(p => p.name == propertyName).value = value;
        }

        public object GetObject(string propertyName) {
            return properties.Find(p => p.name == propertyName).value;
        }

        public T GetProperty<T>(string propertyName) {
            return (T) GetObject(propertyName);
        }
        public void SetProperty<T>(string propertyName, T value) {
            SetObject(propertyName, value);
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

                        //Check
                        takeLink = CheckConditions(link.reverseConditions);
                        if (takeLink) {
                            currentNode = treeData.GetNode(link.endNodeId);
                            frame = 0;
                        }

                        BidirectionalLinkData bidirectionalLink = link as BidirectionalLinkData;
                        if (bidirectionalLink) {
                            takeLink = true;

                            //Check
                            takeLink = CheckConditions(bidirectionalLink.reverseConditions);

                            if (takeLink) {
                                currentNode = treeData.GetNode(bidirectionalLink.startNodeId);
                                frame = 0;
                                break;
                            }
                        }
                    }
                }
            
                
            }
        }

        private bool CheckConditions(List<TreePropertyCondition> conditions) {
            bool takeLink = true;
            foreach (TreePropertyCondition condition in conditions) {
                condition.Process();
                switch (condition.property.type) {
                    case TreePropertyType.Boolean:
                        if (!condition.boolCondition((bool)condition.property.value)) {
                            takeLink = false;
                        }
                        break;
                    case TreePropertyType.Trigger:
                        if ((bool)condition.property.value) {
                            condition.property.value = false;
                        } else {
                            takeLink = false;
                        }
                        break;
                    case TreePropertyType.Integer:
                        if (!condition.intCondition((int)condition.property.value, condition.intValue)) {
                            takeLink = false;
                        }
                        break;
                    case TreePropertyType.Float:
                        if (!condition.floatCondition((float)condition.property.value, condition.floatValue)) {
                            takeLink = false;
                        }
                        break;
                }
                if (!takeLink) break;
            }
            return takeLink;
        }

        private void OnDrawGizmos() {
#if UNITY_EDITOR
            if (currentNode) Handles.Label(transform.position,currentNode.nodeName);
#endif
        }

    }
}
