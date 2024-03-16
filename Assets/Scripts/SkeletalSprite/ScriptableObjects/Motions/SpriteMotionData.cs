using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public abstract class SpriteMotionData : ScriptableObject, ISpriteMotion
    {
        public abstract BoneMotionKeyword Keyword { get; }

        public abstract void ApplyTo(
            ISpriteMotionSet motionSet, int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion);
    }
}
