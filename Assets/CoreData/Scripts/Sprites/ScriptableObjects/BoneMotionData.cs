using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class BoneMotionData : ScriptableObject, IBoneMotion
    {
        public abstract IKeyword Keyword { get; }

        public abstract void ApplyTo(
            IMotionSet motionSet, int animationTime, RogueDirection direction, ref RogueObjSpriteTransform transform, out bool endOfMotion);
    }
}
