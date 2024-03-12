using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    [CreateAssetMenu(menuName = "RoguegardData/Sprite/BoneMotion/Rotatable1to8")]
    public class Rotatable1To8BoneMotionData : BoneMotionData
    {
        //[SerializeField] private KeywordData _keyword = null;
        [SerializeField] private int _pixelsPerUnit = 32;
        [SerializeField] private bool _isLoop = true;
        [SerializeField] private BoneMotionDirectionType _direction = BoneMotionDirectionType.Linear;
        [SerializeField] private List<Item> _items = null;

        public override BoneMotionKeyword Keyword => new BoneMotionKeyword(null);

        public override void ApplyTo(
            IMotionSet motionSet, int animationTime, SpriteDirection direction, ref RogueObjSpriteTransform transform, out bool endOfMotion)
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
        private class Item : IDirectionalBonePoseSource
        {
            [SerializeField] private Sprite _rightSprite;
            private BonePose bonePose;

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

            public BonePose GetBonePose(SpriteDirection direction)
            {
                if (bonePose == null)
                {
                    if (_rightSprite == null) return DefaultBonePoseSource.Instance.GetBonePose(direction);

                    bonePose = new BonePose();
                    var boneSprite = BoneSprite.CreateNF(_rightSprite);
                    var transform = new BoneTransform(boneSprite, _color, true, Vector3.zero, Quaternion.identity, Vector3.one, false, false, false);
                    bonePose.AddBoneTransform(transform, BoneKeyword.Body);
                    bonePose.SetImmutable();
                }

                return bonePose;
            }

            public void Validate()
            {
                if (Rotation.Equals(default)) { _rotation = Quaternion.identity; }
            }
        }
    }
}
