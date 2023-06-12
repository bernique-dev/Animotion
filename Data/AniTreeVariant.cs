using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Animotion;

[CreateAssetMenu(menuName = "Animotion/Animotion Tree Variant")]
public class AniTreeVariant : AniTree {

    [Serializable]
    public class Binding {
        public int nodeId;
        public bool hasMultipleDirections;
        public AniClip clip;
        public AniClipGroup clipGroup;

        public Binding(int id, bool _hasMultipleDirections, AniClip _clip, AniClipGroup _clipGroup) {
            nodeId = id;
            hasMultipleDirections = _hasMultipleDirections;
            clip = _clip;
            clipGroup = _clipGroup;
        }

        public bool IsNull() {
            return (hasMultipleDirections ? (clipGroup == null) : (clip == null));
        }
    }

    public AniTree aniTree;

    public List<Binding> bindings {
        get {
            var nodes = aniTree.GetNodes();
            if (m_bindings == null || m_bindings.Count != nodes.Count) {
                Debug.Log("m_bindings");
                m_bindings = new List<Binding>();
                nodes.ForEach(n => m_bindings.Add(new Binding(n.id, n.hasMultipleDirections, null, null)));
            }
            return m_bindings;
        }
    }
    [SerializeField] private List<Binding> m_bindings;

    public override bool AreNodesSelectable() {
        return false;
    }

    public override List<TreeProperty> GetProperties() {
        return aniTree ? aniTree.GetProperties() : new List<TreeProperty>();
    }

    public override List<AniLink> GetLinks() {
        return aniTree ? aniTree.GetLinks() : new List<AniLink>();
    }

    public override List<AniNode> GetNodes() {
        return aniTree ? aniTree.GetNodes().Select(n => InstantiateNode(n)).ToList() : new List<AniNode>();
    }

    public override AniNode GetNode(int id) {
        var node = aniTree.GetNode(id);
        return aniTree ? InstantiateNode(node) : null;
    }

    private AniNode InstantiateNode(AniNode node) {
        var instantiatedNode = Instantiate(node);
        instantiatedNode.nodeName = node.nodeName;
        instantiatedNode.children = node.children;
        var binding = bindings.Find(b => b.nodeId == node.id);
        if (binding != null) {
            if (binding.hasMultipleDirections) {
                instantiatedNode.clipGroup = binding.IsNull() ? node.clipGroup : binding.clipGroup;
            } else {
                instantiatedNode.clip = binding.IsNull() ? node.clip : binding.clip;
            }
        }
        return instantiatedNode;
    }

    public override AniNode GetRoot() {
        return aniTree ? InstantiateNode(aniTree.GetRoot()) : null;
    }
    public override Dictionary<int, List<AniNode>> GetNodesAndChildren() {
        return aniTree ? aniTree.GetNodesAndChildren() : null;
    }
}
