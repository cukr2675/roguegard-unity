using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class BoneMotionDirectionTypeExtension
    {
        public static RogueDirection Convert(this BoneMotionDirectionType directionType, RogueDirection direction)
        {
            switch (directionType)
            {
                case BoneMotionDirectionType.Linear:
                    return direction;
                case BoneMotionDirectionType.NotBack:
                    if (direction.Degree < 180f) return RogueDirection.FromDegree(360f - direction.Degree);
                    else return direction;
                case BoneMotionDirectionType.DownwardOnly:
                    return RogueDirection.Down;
                default:
                    throw new System.ArgumentException();
            }
        }
    }
}
