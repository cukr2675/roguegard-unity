using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    public ref struct SkeletalSpriteTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public IDirectionalSpritePoseSource PoseSource { get; set; }
        public SpriteDirection Direction { get; set; }

        public static SkeletalSpriteTransform Identity => new SkeletalSpriteTransform(false);

        private SkeletalSpriteTransform(bool flag)
        {
            Position = Vector3.zero;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
            PoseSource = null;
            Direction = SpriteDirection.Down;
        }
    }
}
