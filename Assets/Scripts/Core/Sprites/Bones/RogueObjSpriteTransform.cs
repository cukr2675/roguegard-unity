using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public ref struct RogueObjSpriteTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public IDirectionalBonePoseSource PoseSource { get; set; }
        public RogueDirection Direction { get; set; }

        public static RogueObjSpriteTransform Identity => new RogueObjSpriteTransform(false);

        private RogueObjSpriteTransform(bool flag)
        {
            Position = Vector3.zero;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
            PoseSource = null;
            Direction = RogueDirection.Down;
        }
    }
}
