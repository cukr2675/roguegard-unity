using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public abstract class BoneMotionData : ScriptableObject, IBoneMotion
    {
        public abstract BoneMotionKeyword Keyword { get; }

        public abstract void ApplyTo(
            IMotionSet motionSet, int animationTime, SpriteDirection direction, ref RogueObjSpriteTransform transform, out bool endOfMotion);
    }
}
