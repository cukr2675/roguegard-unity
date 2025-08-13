using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OchalikeSprites
{
    [CreateAssetMenu(menuName = "Ochalike Sprites/Sprite Pose Source/Symmetrical")]
    public class SymmetricalSpritePoseSourceAsset : DirectionalSpritePoseSourceAsset
    {
        [SerializeField] private int _pixelsPerUnit = OchalikeSpritesUtility.DefaultPixelsPerUnit;
        [SerializeField] private List<Item> _items = null;
        [SerializeField] private List<BackItem> _localBacks = null;
        [SerializeField] private List<ReorderItem> _reorders = null;

#if UNITY_EDITOR
        [Header("Editor Only")]
        [SerializeField] internal OchalikeSpriteAsset _previewOchalikeSprite = null; // PropertyDrawer で使用
#endif

        [System.NonSerialized] private ImmutableSymmetricalSpritePoseSource poseSource;

        private void Initialize()
        {
            var pose = new SpritePose();
            foreach (var item in _items)
            {
                pose.AddBoneTransform(item.ToBoneTransform(_pixelsPerUnit), item.BoneName);
            }
            var localBacks = _localBacks.Select(x => x.ToStruct());
            var reorders = _reorders.Select(x => x.ToStruct());
            var boneOrder = new BoneOrder(localBacks, reorders);
            pose.SetBoneOrder(boneOrder);
            pose.SetImmutable();
            poseSource = new ImmutableSymmetricalSpritePoseSource(pose);
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
            poseSource = null;
        }

        [System.Serializable]
        private class Item
        {
            [SerializeField] private BoneKeywordAsset _boneName;
            public BoneKeyword BoneName => _boneName;

            [Tooltip("MorphedBareSprite を上書きする。最初から BareSprite が存在しなければ変化しない")]
            [SerializeField] private bool _hasPoseBareSprite;
            [SerializeField, VisibleBy(nameof(_hasPoseBareSprite))] private BoneSprite _poseBareSprite;
            public BoneSprite PoseBareSprite => _hasPoseBareSprite ? _poseBareSprite : null;

            [Tooltip("MorphedBareColor を上書きする")]
            [SerializeField] private bool _hasPoseBareColor;
            [SerializeField, VisibleBy(nameof(_hasPoseBareColor))] private Color _poseBareColor;
            public Color? PoseBareColor => _hasPoseBareColor ? _poseBareColor : null;

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

            public SpritePoseBoneTransform ToBoneTransform(int pixelsPerUnit)
            {
                return new SpritePoseBoneTransform(
                    PoseBareSprite, PoseBareColor, PixelLocalPosition / pixelsPerUnit, LocalRotation, ScaleOfLocalByLocal,
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
            [SerializeField] private BoneKeywordAsset _name;
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
            [SerializeField] private BoneKeywordAsset _name;
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
