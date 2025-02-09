using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;

namespace Roguegard
{
    public interface ISpriteMotionEffect
    {
        float Order { get; }

        void ApplyTo(ISpriteMotionSet motionSet, IKeyword keyword, int animationTime, RogueDirection direction, ref OchalikeSpriteTransform transform);
    }
}
