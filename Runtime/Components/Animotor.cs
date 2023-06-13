using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    [Serializable]
    public class Animotor : MonoBehaviour {

        private SpriteRenderer spriteRenderer;
        public bool animateOnStart = true;
        public int frame;
        [SerializeField] private bool isTimerRunning;

        public AniNode currentNode {
            get {
                if (m_currentNode == null) {
                    if (aniTree) {
                        m_currentNode = aniTree.GetRoot();
                    }
                }
                return m_currentNode;
            }
            set {
                m_currentNode = value;
                if (value) {
                    currentNodeChildren = currentNode.children.Select(id => aniTree.GetNodes().Find(t => t.id == id)).ToList();
                    List<AniLink> tmpCurrentNodeLinks = currentNode.children.Select(id => aniTree.GetLinks().Find(l => (l.startNodeId == currentNode.id && l.endNodeId == id) || (l.endNodeId == currentNode.id && l.startNodeId == id && l is AniBidirectionalLink))).Distinct().ToList();
                    currentNodeLinks = new List<AniLink>();
                    currentNodeReverseLinks = new List<AniBidirectionalLink>();
                    foreach (AniLink link in tmpCurrentNodeLinks) {
                        AniLink newLink = ScriptableObject.CreateInstance<AniLink>();
                        newLink.name = link.name;
                        newLink.tree = aniTree;
                        if (link.startNodeId == currentNode.id) {
                            newLink.startNodeId = link.startNodeId;
                            newLink.endNodeId = link.endNodeId;
                            foreach (TreePropertyCondition condition in link.conditions) {
                                //Debug.Log(condition.property.name + " " + (String.Join(",",properties.Select(p => p.name).ToArray())));
                                newLink.conditions.Add(condition.Copy(properties.First(p => string.Equals(p.name, condition.property.name))));
                            }
                        }
                        else {
                            newLink.endNodeId = link.startNodeId;
                            newLink.startNodeId = link.endNodeId;
                            foreach (TreePropertyCondition condition in (link as AniBidirectionalLink).reverseConditions) {
                                newLink.conditions.Add(condition.Copy(properties.First(p => string.Equals(p.name, condition.property.name))));
                            }
                        }
                        currentNodeLinks.Add(newLink);
                    }
                }
                //Debug.Log("[" + gameObject + " | " + animotionClip + "]");
            }
        }
        public AniNode m_currentNode;

        public List<AniNode> currentNodeChildren;
        public List<AniLink> currentNodeLinks;
        public List<AniBidirectionalLink> currentNodeReverseLinks;

        public AniTree aniTree;
        // Hideable
        [SerializeField] private AniDirection m_aniDirection = AniDirection.Right;
        public AniClip animotionClip {
            get {
                if (currentNode) {
                    //Debug.Log("currentNode");
                    if (currentNode.hasMultipleDirections) {
                        //Debug.Log("Multiple");
                        return currentNode.GetAnimotionClip(m_aniDirection);
                    } else {
                        //Debug.Log("Single");
                        return currentNode.GetAnimotionClip();
                    }
                }
                return null;
                //return currentNode ? currentNode.hasMultipleDirections ?  : currentNode.GetAnimotionClip() : null;
            }
        }

        public AniDirection direction {
            get {
                return m_aniDirection;
            }
        }

        public List<TreeProperty> properties {
            get {
                return m_properties != null ? m_properties : aniTree.GetProperties();
            }
        }


        [SerializeField] private List<TreeProperty> m_properties;

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (animotionClip) {
                spriteRenderer.sprite = animotionClip.GetFrame(0).sprite;
            }
            isTimerRunning = animateOnStart;
            if (aniTree) {
                UpdateTree();
            }
        }

        public void UpdateTree() {
            UpdateProperties();
            currentNode = aniTree ? aniTree.GetRoot() : null;
        }

        public void UpdateProperties() {
            m_properties = new List<TreeProperty>();
            if (aniTree) {
                foreach (TreeProperty treeProperty in aniTree.GetProperties()) {
                    TreeProperty newTreeProperty = ScriptableObject.CreateInstance<TreeProperty>();
                    newTreeProperty.SetValues(treeProperty);
                    m_properties.Add(newTreeProperty);
                }
            }
        }

        public void SetDirection(Vector2 delta) {
            if (currentNode) {
                if (currentNode.hasMultipleDirections) {
                    SetDirection(delta.GetAniDirection(currentNode.clipGroup.mode));
                }
                else {
                    SetDirection(delta.GetAniDirection());
                }
            }
        }

        public void SetDirection(AniDirection _aniDirection) {
            if (currentNode.hasMultipleDirections) {
                m_aniDirection = _aniDirection.GetVector2().GetAniDirection(currentNode.clipGroup.mode);
            }
            else {
                m_aniDirection = _aniDirection;
            }
        }

        public void SetObject(string propertyName, object value) {
            //Debug.Log("[" + gameObject.name + "] " + propertyName + " -> "  + value);
            properties.Find(p => p.name == propertyName).value = value;
        }

        public object GetObject(string propertyName) {
            return properties.Find(p => p.name == propertyName).value;
        }

        public T GetProperty<T>(string propertyName) {
            return (T)GetObject(propertyName);
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
            //Debug.Log(gameObject);
            //Debug.LogError("FixedUpdate " + (isTimerRunning ? "[RUNNING]" : "[PAUSE]") + " " + (animateOnStart ? "[START]" : "[PAS START]") + " " + (animotionClip ? "[CLIP]" : "[PAS CLIP]"));
            if (isTimerRunning) {
                if (animotionClip) {
                    frame += 1;
                    AniFrame frameData = animotionClip.GetLastFrame(frame);
                    if (frameData == null) Debug.Log(frame + " (" + frame + ")/" + animotionClip.length);
                    spriteRenderer.sprite = frameData.sprite;
                    if (frame >= animotionClip.length) {
                        frame = 0;
                        isTimerRunning = animotionClip.loop;
                    }
                }
                else {
                    frame = 0;
                }
            }
            if (currentNode) {
                if (!currentNode.waitForEnd || (frame == 0)) {
                    CheckLinks(currentNodeLinks);
                }
            }
        }


        private void CheckLinks(List<AniLink> links) {
            foreach (AniLink link in links) {
                if (link) {
                    bool takeLink = true;
                    //Check
                    takeLink = CheckConditions(link.conditions);
                    if (takeLink) {
                        currentNode = aniTree.GetNode(link.endNodeId);
                        frame = 0;
                        return;
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
                        if (!((bool)condition.property.value)) {
                            takeLink = false;
                        }
                        condition.property.value = false;
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
            if (currentNode) Handles.Label(transform.position, currentNode.nodeName);
#endif
        }

    }
}
