using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public interface IRogueSpriteMotion : ISpriteMotion
    {
        IKeyword Keyword { get; }

        void ApplyTo(ISpriteMotionSet motionSet, int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion);
    }
}
