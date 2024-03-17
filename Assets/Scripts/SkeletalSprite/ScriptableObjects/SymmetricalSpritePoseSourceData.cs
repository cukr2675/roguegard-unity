using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace SkeletalSprite
{
    [CreateAssetMenu(menuName = "SkeletalSprite/SpritePoseSource/Symmetrical")]
    public class SymmetricalSpritePoseSourceData : DirectionalSpritePoseSourceData
    {
        [SerializeField] private int _pixelsPerUnit = SkeletalSpriteUtility.PixelsPerUnit;
        [SerializeField] private List<Item> _items = null;
        [SerializeField] private List<BackItem> _localBacks = null;
        [SerializeField] private List<ReorderItem> _reorders = null;

        [System.NonSerialized] private ImmutableSymmetricalSpritePoseSource poseSource;

        private void Initialize()
        {
            var spritePose = new SpritePose();
            foreach (var item in _items)
            {
                spritePose.AddBoneTransform(item.ToBoneTransform(_pixelsPerUnit), item.BoneName);
            }
            var localBacks = _localBacks.Select(x => x.ToStruct());
            var reorders = _reorders.Select(x => x.ToStruct());
            var boneOrder = new BoneOrder(localBacks, reorders);
            spritePose.SetBoneOrder(boneOrder);
            spritePose.SetImmutable();
            poseSource = new ImmutableSymmetricalSpritePoseSource(spritePose);
        }

        public override SpritePose GetSpritePose(SpriteDirection direction)
        {
            if (poseSource == null) { Initialize(); }

            return poseSource.GetSpritePose(direction);
        }

        private void OnValidate()
        {
            foreach (var item in _items)
            {
                item?.Validate();
            }
        }

        [System.Serializable]
        private class Item
        {
            [SerializeField] private BoneKeywordData _boneName;
            public BoneKeyword BoneName => _boneName;

            [SerializeField] private bool _overridesSourceSprite;
            public bool OverridesSourceSprite { get => _overridesSourceSprite; set => _overridesSourceSprite = value; }

            [SerializeField] private BoneSprite _boneSprite;
            public BoneSprite BoneSprite { get => _boneSprite; set => _boneSprite = value; }

            [SerializeField] private bool _overridesSourceColor;
            public bool OverridesSourceColor { get => _overridesSourceColor; set => _overridesSourceColor = value; }

            [SerializeField] private Color _color;
            public Color Color { get => _color; set => _color = value; }

            [SerializeField] private Vector3 _pixelLocalPosition;
            public Vector3 PixelLocalPosition { get => _pixelLocalPosition; set => _pixelLocalPosition = value; }

            [SerializeField] private Quaternion _localRotation;
            public Quaternion LocalRotation { get => _localRotation; set => _localRotation = value; }

            [SerializeField] private Vector3 _scaleOfLocalByLocal;
            public Vector3 ScaleOfLocalByLocal { get => _scaleOfLocalByLocal; set => _scaleOfLocalByLocal = value; }

            [SerializeField] private bool _transformsInRootParent;
            public bool TransformsInRootParent { get => _transformsInRootParent; set => _transformsInRootParent = value; }

            [SerializeField] private bool _localMirrorX;
            public bool LocalMirrorX { get => _localMirrorX; set => _localMirrorX = value; }

            [SerializeField] private bool _localMirrorY;
            public bool LocalMirrorY { get => _localMirrorY; set => _localMirrorY = value; }

            public BoneTransform ToBoneTransform(int pixelsPerUnit)
            {
                return new BoneTransform(
                    OverridesSourceSprite ? BoneSprite : null, Color, OverridesSourceColor,
                    PixelLocalPosition / pixelsPerUnit, LocalRotation, ScaleOfLocalByLocal,
                    TransformsInRootParent, LocalMirrorX, LocalMirrorY);
            }

            public void Validate()
            {
                if (_localRotation.Equals(default)) { _localRotation = Quaternion.identity; }
            }
        }

        [System.Serializable]
        private class BackItem
        {
            [SerializeField] private BoneKeywordData _name;
            public BoneKeyword Name => _name;

            [SerializeField] private BoneBack.Type _localBack;
            public BoneBack.Type LocalBack { get => _localBack; set => _localBack = value; }

            public BoneBack ToStruct()
            {
                return new BoneBack(Name, LocalBack);
            }
        }

        [System.Serializable]
        private class ReorderItem
        {
            [SerializeField] private BoneKeywordData _name;
            public BoneKeyword Name => _name;

            [SerializeField] private BoneReorder.Type _reorder;
            public BoneReorder.Type Reorder { get => _reorder; set => _reorder = value; }

            public BoneReorder ToStruct()
            {
                return new BoneReorder(Name, Reorder);
            }
        }
    }
}
