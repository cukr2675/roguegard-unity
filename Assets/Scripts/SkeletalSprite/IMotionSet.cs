using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public interface IMotionSet
    {
        void GetPose(BoneMotionKeyword keyword, int animationTime, SpriteDirection direction, ref RogueObjSpriteTransform transform, out bool endOfMotion);
    }
}
