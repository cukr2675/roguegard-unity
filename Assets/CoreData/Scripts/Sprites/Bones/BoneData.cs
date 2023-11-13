using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/Sprite/Bone")]
    public class BoneData : ScriptableObject
    {
        [SerializeField] private List<Node> _nodes = new List<Node>();

        public void Add(Node node)
        {
            _nodes.Add(node);
        }

        public void ClearNodes()
        {
            _nodes.Clear();
        }

        public BoneNodeBuilder CreateBoneNodeBuilder(Color color)
        {
            return Recursion(_nodes[0]);

            BoneNodeBuilder Recursion(Node boneNode)
            {
                var bone = new Bone(boneNode, color);
                var children = new List<BoneNodeBuilder>();
                var startIndex = _nodes.IndexOf(boneNode) + 1;
                for (int i = startIndex; i < _nodes.Count; i++)
                {
                    var itemNode = _nodes[i];
                    if (itemNode.ParentBoneName != boneNode.BoneName) continue;

                    var child = Recursion(itemNode);
                    children.Add(child);
                }
                return new BoneNodeBuilder(bone, children);
            }
        }

        private void OnValidate()
        {
            foreach (var node in _nodes)
            {
                if (node.LocalRotation.Equals(default)) node.LocalRotation = Quaternion.identity;
            }
        }

        [System.Serializable]
        public class Node
        {
            [SerializeField] private KeywordData _boneName = null;
            public KeywordData BoneName { get => _boneName; set => _boneName = value; }

            [SerializeField] private KeywordData _parentBoneName = null;
            public KeywordData ParentBoneName { get => _parentBoneName; set => _parentBoneName = value; }

            [Space]

            [SerializeField] private ColorRangedBoneSprite _sprite = null;
            public ColorRangedBoneSprite Sprite { get => _sprite; set => _sprite = value; }

            [SerializeField] private bool _overridesBaseColor = true;
            public bool OverridesBaseColor { get => _overridesBaseColor; set => _overridesBaseColor = value; }

            [SerializeField] private bool _flipX = false;
            public bool FlipX { get => _flipX; set => _flipX = value; }

            [SerializeField] private bool _flipY = false;
            public bool FlipY { get => _flipY; set => _flipY = value; }

            [SerializeField] private Vector3 _pixelLocalPosition = Vector3.zero;
            public Vector3 PixelLocalPosition { get => _pixelLocalPosition; set => _pixelLocalPosition = value; }

            [SerializeField] private Quaternion _localRotation = Quaternion.identity;
            public Quaternion LocalRotation { get => _localRotation; set => _localRotation = value; }

            [SerializeField] private Vector3 _scaleOfLocalByLocal = Vector3.one;
            public Vector3 ScaleOfLocalByLocal { get => _scaleOfLocalByLocal; set => _scaleOfLocalByLocal = value; }

            [SerializeField] private float _normalOrderInParent = 0f;
            public float NormalOrderInParent { get => _normalOrderInParent; set => _normalOrderInParent = value; }

            [SerializeField] private float _backOrderInParent = 0f;
            public float BackOrderInParent { get => _backOrderInParent; set => _backOrderInParent = value; }
        }

        private class Bone : IBone
        {
            public IKeyword Name { get; set; }
            public BoneSprite Sprite { get; set; }
            public Color Color { get; set; }
            public bool OverridesBaseColor { get; set; }
            public bool FlipX { get; set; }
            public bool FlipY { get; set; }
            public Vector3 LocalPosition { get; set; }
            public Quaternion LocalRotation { get; set; }
            public Vector3 ScaleOfLocalByLocal { get; set; }
            public float NormalOrderInParent { get; set; }
            public float BackOrderInParent { get; set; }

            public Bone(Node node, Color color)
            {
                Name = node.BoneName;
                var colorRange = RogueColorUtility.GetColorRange(color);
                Sprite = node.Sprite.GetSprite(colorRange);
                Color = color;
                OverridesBaseColor = node.OverridesBaseColor;
                FlipX = node.FlipX;
                FlipY = node.FlipY;
                LocalPosition = node.PixelLocalPosition / RoguegardSettings.PixelPerUnit;
                LocalRotation = node.LocalRotation;
                ScaleOfLocalByLocal = node.ScaleOfLocalByLocal;
                NormalOrderInParent = node.NormalOrderInParent;
                BackOrderInParent = node.BackOrderInParent;
            }
        }
    }
}
