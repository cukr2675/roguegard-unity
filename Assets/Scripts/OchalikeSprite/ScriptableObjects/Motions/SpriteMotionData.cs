using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprite
{
    public abstract class SpriteMotionData : ScriptableObject, ISpriteMotion
    {
        public abstract void ApplyTo(int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion);
    }
}
