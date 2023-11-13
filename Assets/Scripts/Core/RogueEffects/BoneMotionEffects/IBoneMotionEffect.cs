using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IBoneMotionEffect
    {
        float Order { get; }

        void ApplyTo(IMotionSet motionSet, IKeyword keyword, int animationTime, RogueDirection direction, ref RogueObjSpriteTransform transform);
    }
}
