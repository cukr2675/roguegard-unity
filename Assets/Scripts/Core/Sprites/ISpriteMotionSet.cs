using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;

namespace Roguegard
{
    public interface ISpriteMotionSet
    {
        void GetPose(IKeyword keyword, int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion);
    }
}
