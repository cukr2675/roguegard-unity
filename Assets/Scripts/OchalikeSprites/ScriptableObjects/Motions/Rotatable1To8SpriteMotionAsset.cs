using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    [CreateAssetMenu(menuName = "Ochalike Sprites/Sprite Motion/Rotatable 1 to 8")]
    public class Rotatable1To8SpriteMotionAsset : SpriteMotionAsset
    {
        [SerializeField] private int _pixelsPerUnit = OchalikeSpritesUtility.DefaultPixelsPerUnit;
        [SerializeField] private bool _isLoop = true;
        [SerializeField] private SpriteMotionDirection _direction = SpriteMotionDirection.Linear;
        [SerializeField] private List<Item> _items = null;
        
#if UNITY_EDITOR
        [Header("Editor Only")]
        [SerializeField] internal OchalikeSpriteAsset _previewOchalikeSprite = null; // PropertyDrawer で使用する
#endif

        public override void ApplyTo(int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion)
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
            var first = false;
            foreach (var item in _items)
            {
                sum += item.Wait;
                if (index < sum)
                {
                    current = item;
                    if (index == sum - item.Wait + 1 || item.Wait == 1) { first = true; }
                    break;
                }
            }

            var degree = direction.Convert(_direction).Degree + current.Degree;
            var degreeRotation = Quaternion.Euler(0f, 0f, degree);
            transform.Position = (current.PixelPosition + degreeRotation * current.PixelRotatablePosition) / _pixelsPerUnit;
            transform.Rotation = current.Rotation * degreeRotation;
            transform.Scale = current.Scale;
            transform.PoseSource = current;
            transform.Direction = SpriteDirection.FromDegree(degree);
            if (first) { transform.Play = current.Play; } // 切り替わった瞬間だけ再生
            endOfMotion = index >= sumWait - 1;
        }

        protected virtual void OnValidate()
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
            private SpritePose pose;

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

            [SerializeField] private string _play;
            public string Play => _play;

            [SerializeField] private int _wait;
            public int Wait => _wait;

            public SpritePose GetSpritePose(SpriteDirection direction)
            {
                if (pose == null)
                {
                    if (_rightSprite == null) return DefaultSpritePoseSource.Instance.GetSpritePose(direction);

                    pose = new SpritePose();
                    var boneSprite = BoneSprite.CreateNF(_rightSprite);
                    var transform = new SpritePoseBoneTransform(boneSprite, _color, Vector3.zero, Quaternion.identity, Vector3.one, false, false, false);
                    pose.AddBoneTransform(transform, BoneKeyword.Body);
                    pose.SetImmutable();
                }

                return pose;
            }

            public void Validate()
            {
                if (Rotation.Equals(default)) { _rotation = Quaternion.identity; }
            }
        }
    }
}
