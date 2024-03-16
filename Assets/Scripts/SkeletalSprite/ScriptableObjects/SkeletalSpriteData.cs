using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    [CreateAssetMenu(menuName = "SkeletalSprite/Bone")]
    public class SkeletalSpriteData : ScriptableObject
    {
        [SerializeField] private List<Node> _nodes = new List<Node>();

        public Node this[int index]
        {
            get => _nodes[index];
            set => _nodes[index] = value;
        }

        public int Count => _nodes.Count;

        public void Add(Node node)
        {
            _nodes.Add(node);
        }

        public void ClearNodes()
        {
            _nodes.Clear();
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
            [SerializeField] private BoneKeywordData _boneName = null;
            public BoneKeywordData BoneName { get => _boneName; set => _boneName = value; }

            [SerializeField] private BoneKeywordData _parentBoneName = null;
            public BoneKeywordData ParentBoneName { get => _parentBoneName; set => _parentBoneName = value; }

            [Space]

            [SerializeField] private int _pixelsPerUnit = 32;
            public int PixelsPerUnit { get => _pixelsPerUnit; set => _pixelsPerUnit = value; }

            [SerializeField] private float _lightDarkThreshold = 0.2f;
            public float LightDarkThreshold { get => _lightDarkThreshold; set => _lightDarkThreshold = value; }

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
    }
}
