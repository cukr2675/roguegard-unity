using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Sprite/BoneMotion/RotatableLoop")]
    public class RotatableLoopBoneMotionData : BoneMotionData
    {
        //[SerializeField] private KeywordData _keyword = null;
        [SerializeField] private int _loopCount = 0;
        [SerializeField] private BoneMotionDirectionType _direction = BoneMotionDirectionType.Linear;
        [SerializeField] private List<Item> _items = null;

        public override IKeyword Keyword => null;

        public override void ApplyTo(
            IMotionSet motionSet, int animationTime, RogueDirection direction, ref RogueObjSpriteTransform transform, out bool endOfMotion)
        {
            var oneLoopWait = 0;
            foreach (var item in _items)
            {
                oneLoopWait += item.Wait;
            }
            var sumWait = oneLoopWait * _loopCount;

            var index = Mathf.Min(animationTime, sumWait - 1);
            var sum = 0;
            Item current = null;
            foreach (var item in _items)
            {
                sum += item.Wait;
                if ((index % oneLoopWait) < sum)
                {
                    current = item;
                    break;
                }
            }

            var degree = _direction.Convert(direction).Degree + current.Degree;
            var degreeRotation = Quaternion.Euler(0f, 0f, degree);
            transform.Position = (current.PixelPosition + degreeRotation * current.PixelRotatablePosition) / RoguegardSettings.PixelsPerUnit;
            transform.Rotation = current.Rotation;
            transform.Scale = current.Scale;
            transform.PoseSource = current.PoseSource;
            transform.Direction = RogueDirection.FromDegree(degree);
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
        private class Item
        {
            [SerializeField] private DirectionalBonePoseSourceData _poseSource;
            public IDirectionalBonePoseSource PoseSource => _poseSource;

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

            public void Validate()
            {
                if (Rotation.Equals(default)) { _rotation = Quaternion.identity; }
            }
        }
    }
}
