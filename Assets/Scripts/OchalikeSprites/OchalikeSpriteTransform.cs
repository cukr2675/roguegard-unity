using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public ref struct OchalikeSpriteTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public IDirectionalSpritePoseSource PoseSource { get; set; }
        public SpriteDirection Direction { get; set; }

        public static OchalikeSpriteTransform Identity => new OchalikeSpriteTransform(false);

        private OchalikeSpriteTransform(bool flag)
        {
            Position = Vector3.zero;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
            PoseSource = null;
            Direction = SpriteDirection.Down;
        }
    }
}
