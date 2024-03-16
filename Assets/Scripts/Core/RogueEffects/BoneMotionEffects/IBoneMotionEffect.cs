using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public interface IBoneMotionEffect
    {
        float Order { get; }

        void ApplyTo(ISpriteMotionSet motionSet, IKeyword keyword, int animationTime, RogueDirection direction, ref SkeletalSpriteTransform transform);
    }
}
