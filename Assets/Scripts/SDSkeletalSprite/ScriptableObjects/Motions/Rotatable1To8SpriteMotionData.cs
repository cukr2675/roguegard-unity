using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    [CreateAssetMenu(menuName = "SDSkeletalSprite/SpriteMotion/Rotatable1to8")]
    public class Rotatable1To8SpriteMotionData : SpriteMotionData
    {
        [SerializeField] private int _pixelsPerUnit = SDSSpriteUtility.DefaultPixelsPerUnit;
        [SerializeField] private bool _isLoop = true;
        [SerializeField] private SpriteMotionDirectionType _direction = SpriteMotionDirectionType.Linear;
        [SerializeField] private List<Item> _items = null;

        public override void ApplyTo(int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion)
        {
            var sumWait = 0;
            foreach (var item in _items)
            {
                sumWait += item.Wait;
            }

            int index;
            if (_isLoop) index = animationTime % sumWait;
            else index = Mathf.Min(animationTime, sumWait - 1);
            var sum = 0;
            Item current = null;
            foreach (var item in _items)
            {
                sum += item.Wait;
                if (index < sum)
                {
                    current = item;
                    break;
                }
            }

            var degree = _direction.Convert(direction).Degree + current.Degree;
            var degreeRotation = Quaternion.Euler(0f, 0f, degree);
            transform.Position = (current.PixelPosition + degreeRotation * current.PixelRotatablePosition) / _pixelsPerUnit;
            transform.Rotation = current.Rotation * degreeRotation;
            transform.Scale = current.Scale;
            transform.PoseSource = current;
            transform.Direction = SpriteDirection.FromDegree(degree);
            endOfMotion = index >= sumWait - 1;
        }

        private void OnValidate()
        {
            foreach (var item in _items)
            {
                item.Validate();
            }
        }

        [System.Serializable]
        private class Item : IDirectionalSpritePoseSource
        {
            [SerializeField] private Sprite _rightSprite;
            private SpritePose spritePose;

            [SerializeField] private Color _color;

            [SerializeField] private Vector3 _pixelPosition;
            public Vector3 PixelPosition => _pixelPosition;

            [SerializeField] private Vector3 _pixelRotatablePosition;
            public Vector3 PixelRotatablePosition => _pixelRotatablePosition;

            [SerializeField] private Quaternion _rotation;
            public Quaternion Rotation => _rotation;

            [SerializeField] private Vector3 _scale;
            public Vector3 Scale => _scale;

            [SerializeField] private float _degree;
            public float Degree => _degree;

            [SerializeField] private int _wait;
            public int Wait => _wait;

            public SpritePose GetSpritePose(SpriteDirection direction)
            {
                if (spritePose == null)
                {
                    if (_rightSprite == null) return DefaultSpriteMotionPoseSource.Instance.GetSpritePose(direction);

                    spritePose = new SpritePose();
                    var boneSprite = BoneSprite.CreateNF(_rightSprite);
                    var transform = new BoneTransform(boneSprite, _color, true, Vector3.zero, Quaternion.identity, Vector3.one, false, false, false);
                    spritePose.AddBoneTransform(transform, BoneKeyword.Body);
                    spritePose.SetImmutable();
                }

                return spritePose;
            }

            public void Validate()
            {
                if (Rotation.Equals(default)) { _rotation = Quaternion.identity; }
            }
        }
    }
}
