using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    [Serializable]
    public class Animotor : MonoBehaviour {

        private SpriteRenderer spriteRenderer {
            get {
                if (m_spriteRenderer == null) {
                    m_spriteRenderer = GetComponent<SpriteRenderer>();
                }
                return m_spriteRenderer;
            }
        }
        private SpriteRenderer m_spriteRenderer;

        public bool animateOnStart = true;
        public int frame;
        [SerializeField] private bool isTimerRunning;

        private List<LinkedAnimotor> linkedAnimotors {
            get {
                if (m_linkedAnimotors == null) { m_linkedAnimotors = new List<LinkedAnimotor>(); }
                return m_linkedAnimotors;
            }
        }
        [SerializeField] private List<LinkedAnimotor> m_linkedAnimotors;

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

                    List<AniLink> tmpCurrentNodeLinks = currentNodeChildren.Select(child => aniTree.GetLinks().Find(l => (l.startNodeId == currentNode.id && l.endNodeId == child.id) || (l.endNodeId == currentNode.id && l.startNodeId == child.id && l is AniBidirectionalLink))).Distinct().ToList();
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
                        } else {
                            newLink.endNodeId = link.startNodeId;
                            newLink.startNodeId = link.endNodeId;
                            foreach (TreePropertyCondition condition in (link as AniBidirectionalLink).reverseConditions) {
                                newLink.conditions.Add(condition.Copy(properties.First(p => string.Equals(p.name, condition.property.name))));
                            }
                        }
                        currentNodeLinks.Add(newLink);
                    }

                    var globalNodes = aniTree.GetGlobalNodes().ToList();
                    var globalLinks = aniTree.GetLinks().Where(l => globalNodes.Any(node => node.id == l.startNodeId)).ToList();
                    foreach (AniLink globalLink in globalLinks) {
                        AniLink newLink = ScriptableObject.CreateInstance<AniLink>();
                        newLink.name = globalLink.name;
                        newLink.tree = aniTree;
                        newLink.startNodeId = currentNode.id;
                        newLink.endNodeId = globalLink.endNodeId;
                        foreach (TreePropertyCondition condition in globalLink.conditions) {
                            //Debug.Log(condition.property.name + " " + (String.Join(",",properties.Select(p => p.name).ToArray())));
                            newLink.conditions.Add(condition.Copy(properties.First(p => string.Equals(p.name, condition.property.name))));
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
                        var _animotionClip = currentNode.GetAnimotionClip(m_aniDirection);
                        spriteRenderer.flipX = false;
                        if (_animotionClip is null) {
                            var mirroredDirection = direction.GetMirroredAniDirection();
                            if (direction != mirroredDirection)  {
                                _animotionClip = currentNode.GetAnimotionClip(mirroredDirection);
                                if (_animotionClip is not null) {
                                    spriteRenderer.flipX = true;
                                }
                            }
                        }
                        return _animotionClip;
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
                if (m_properties != null) {
                    if (m_properties.All(p => p is not null)) {
                        return m_properties;
                    }
                }
                return GetTreeProperties();
            }
        }
        [SerializeField] private List<TreeProperty> m_properties;



        private void Awake() {
            if (animotionClip) {
                spriteRenderer.sprite = animotionClip.GetFrame(0).sprite;
            }
            isTimerRunning = animateOnStart;
            if (aniTree) {
                UpdateTree();
            }
            SetDirection(AniDirection.Left);
        }

        public virtual Animotor GetAnimotor() {
            return this;
        }

        public void LinkAnimotor(LinkedAnimotor linkedAnimotor) {
            linkedAnimotors.Add(linkedAnimotor);
        }

        public void UnlinkAnimotor(LinkedAnimotor linkedAnimotor) {
            linkedAnimotors.Remove(linkedAnimotor);
        }

        public List<LinkedAnimotor> GetLinkedAnimotors() {
            return linkedAnimotors;
        }

        public void UpdatePropertyValues() {
            foreach (var property in properties) {
                UpdatePropertyValue(property.name, property.value);
            }
        }

        public bool HasProperty(string propertyName) {
            return properties.Any(p => p.name == propertyName);
        }

        public virtual void UpdatePropertyValue(string propertyName, object value, bool callChildren = true, bool callParent = true) {
            var propertyByName = properties.Find(p => p.name == propertyName);
            if (propertyByName != null) {
                propertyByName.value = value;
            } else {
                throw new ArgumentException($"No property found with name = '{propertyByName}'");
            }
            if (callChildren) {
                foreach (var linkedAnimotor in linkedAnimotors) {
                    if (linkedAnimotor.HasProperty(propertyName)) {
                        linkedAnimotor.UpdatePropertyValue(propertyName, value, true, false);
                    }
                }
            }
        }

        public void UpdateTree() {
            UpdateProperties();
            currentNode = aniTree ? aniTree.GetRoot() : null;
        }

        public void UpdateProperties() {
            m_properties = new List<TreeProperty>();
            if (aniTree) {
                foreach (TreeProperty treeProperty in GetTreeProperties()) {
                    TreeProperty newTreeProperty = ScriptableObject.CreateInstance<TreeProperty>();
                    newTreeProperty.SetValues(treeProperty);
                    m_properties.Add(newTreeProperty);
                }
            }
        }

        public virtual List<TreeProperty> GetTreeProperties() {
            return aniTree.GetProperties();
        }

        public void SetDirection(Vector2 delta) {
            if (currentNode) {
                if (currentNode.hasMultipleDirections) {
                    SetDirection(delta.GetAniDirection(currentNode.clipGroup.mode));
                } else {
                    SetDirection(delta.GetAniDirection());
                }
            }
        }

        public void SetDirection(AniDirection _aniDirection) {
            if (currentNode.hasMultipleDirections) {
                m_aniDirection = _aniDirection.GetVector2().GetAniDirection(currentNode.clipGroup.mode);
            } else {
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
                    if (frameData == null)
                        Debug.Log(frame + " (" + frame + ")/" + animotionClip.length);
                    spriteRenderer.sprite = frameData.sprite;
                    if (frame >= animotionClip.length) {
                        frame = 0;
                        isTimerRunning = animotionClip.loop;
                    }
                } else {
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
                if (!takeLink)
                    break;
            }
            return takeLink;
        }

        private void OnDrawGizmos() {
#if UNITY_EDITOR
            if (currentNode)
                Handles.Label(transform.position, currentNode.nodeName);
#endif
        }

    }
}
