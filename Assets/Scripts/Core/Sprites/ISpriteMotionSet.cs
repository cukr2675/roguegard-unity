using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprite;

namespace Roguegard
{
    public interface ISpriteMotionSet
    {
        void GetPose(IKeyword keyword, int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion);
    }
}
