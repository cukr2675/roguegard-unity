using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IMotionSet
    {
        void GetPose(IKeyword keyword, int animationTime, RogueDirection direction, ref RogueObjSpriteTransform transform, out bool endOfMotion);
    }
}
