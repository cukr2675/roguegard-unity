using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    [CreateAssetMenu(menuName = "Ochalike Sprites/Ochalike Sprite")]
    public class OchalikeSpriteData : ScriptableObject
    {
        [SerializeField] private int _pixelsPerUnit = OchalikeSpritesUtility.DefaultPixelsPerUnit;
        public int PixelsPerUnit { get => _pixelsPerUnit; set => _pixelsPerUnit = value; }

        [SerializeField] private float _lightDarkThreshold = OchalikeSpritesUtility.LightDarkThreshold;
        public float LightDarkThreshold { get => _lightDarkThreshold; set => _lightDarkThreshold = value; }

        [SerializeField] private List<Bone> _bones = new List<Bone>();

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

        public OchalikeBone CreateBone(Color color, float brightness)
        {
            var bright = brightness >= _lightDarkThreshold;
            return Recursion(0);

            OchalikeBone Recursion(int index)
            {
                var bone = _bones[index];
                var result = bone.ToBone(color, bright, _pixelsPerUnit);
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
            [SerializeField] private BoneKeywordData _boneName = null;
            public BoneKeywordData BoneName { get => _boneName; set => _boneName = value; }

            [SerializeField] private BoneKeywordData _parentBoneName = null;
            public BoneKeywordData ParentBoneName { get => _parentBoneName; set => _parentBoneName = value; }

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

            public OchalikeBone ToBone(Color color, bool bright, int pixelsPerUnit)
            {
                var bone = new OchalikeBone();
                bone.Name = _boneName;
                bone.Sprite = _sprite.GetSprite(bright);
                bone.Color = color;
                bone.OverridesBaseColor = _overridesBaseColor;
                bone.FlipX = _flipX;
                bone.FlipY = _flipY;
                bone.LocalPosition = _pixelLocalPosition / pixelsPerUnit;
                bone.LocalRotation = _localRotation;
                bone.ScaleOfLocalByLocal = _scaleOfLocalByLocal;
                bone.NormalOrderInParent = _normalOrderInParent;
                bone.BackOrderInParent = _backOrderInParent;
                return bone;
            }
        }
    }
}
