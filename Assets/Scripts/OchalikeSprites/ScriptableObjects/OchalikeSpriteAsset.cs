using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    [CreateAssetMenu(menuName = "Ochalike Sprites/Ochalike Sprite")]
    public class OchalikeSpriteAsset : ScriptableObject
    {
        [SerializeField] private int _pixelsPerUnit = OchalikeSpritesUtility.DefaultPixelsPerUnit;
        public int PixelsPerUnit { get => _pixelsPerUnit; set => _pixelsPerUnit = value; }

        [SerializeField] private List<Bone> _bones = new();

        public Bone this[int index]
        {
            get => _bones[index];
            set => _bones[index] = value;
        }

        public int Count => _bones.Count;

        public void Add(Bone bone)
        {
            _bones.Add(bone);
        }

        public void ClearBones()
        {
            _bones.Clear();
        }

        public OchalikeBone CreateOchalikeSpriteWithHairColor(Color bareColor, OchalikeMorph morph)
        {
            var morphItem = morph.GetSprite(BoneKeyword.Hair);
            var hairColor = morphItem.MorphBareColor ?? Color.black;
            var useDarkOutline = OchalikeSpritesUtility.IsSimilarToLightOutline(hairColor);
            return CreateOchalikeSprite(bareColor, useDarkOutline);
        }

        public OchalikeBone CreateOchalikeSprite(Color bareColor, bool useDarkOutline)
        {
            return Recursion(0);

            OchalikeBone Recursion(int index)
            {
                var bone = _bones[index];
                var result = bone.ToBone(bareColor, useDarkOutline, _pixelsPerUnit);
                var startIndex = index + 1;
                for (int i = startIndex; i < _bones.Count; i++)
                {
                    var itemBone = _bones[i];
                    if (itemBone.ParentBoneName != bone.BoneName) continue;

                    var child = Recursion(i);
                    result.Children.Add(child);
                }
                return result;
            }
        }

        private void OnValidate()
        {
            foreach (var bone in _bones)
            {
                if (bone.LocalRotation.Equals(default)) bone.LocalRotation = Quaternion.identity;
            }
        }

        [System.Serializable]
        public class Bone
        {
            [SerializeField] private BoneKeywordAsset _boneName = null;
            public BoneKeywordAsset BoneName { get => _boneName; set => _boneName = value; }

            [SerializeField] private BoneKeywordAsset _parentBoneName = null;
            public BoneKeywordAsset ParentBoneName { get => _parentBoneName; set => _parentBoneName = value; }

            [Space]

            [SerializeField] private ColorRangedBoneSprite _bareSprite = null;
            public ColorRangedBoneSprite BareSprite { get => _bareSprite; set => _bareSprite = value; }

            [SerializeField] private bool _overridesOnDefaultColor = true;
            public bool OverridesOnDefaultColor { get => _overridesOnDefaultColor; set => _overridesOnDefaultColor = value; }

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

            public OchalikeBone ToBone(Color bareColor, bool useDarkOutline, int pixelsPerUnit)
            {
                return new OchalikeBone
                {
                    Name = _boneName,
                    BareSprite = _bareSprite.GetSprite(useDarkOutline),
                    BareColor = bareColor,
                    OverridesOnDefaultColor = _overridesOnDefaultColor,
                    FlipX = _flipX,
                    FlipY = _flipY,
                    LocalPosition = _pixelLocalPosition / pixelsPerUnit,
                    LocalRotation = _localRotation,
                    ScaleOfLocalByLocal = _scaleOfLocalByLocal,
                    NormalOrderInParent = _normalOrderInParent,
                    BackOrderInParent = _backOrderInParent
                };
            }
        }
    }
}
