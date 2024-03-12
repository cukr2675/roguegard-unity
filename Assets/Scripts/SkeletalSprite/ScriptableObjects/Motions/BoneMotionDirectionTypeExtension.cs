using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public static class BoneMotionDirectionTypeExtension
    {
        public static SpriteDirection Convert(this BoneMotionDirectionType directionType, SpriteDirection direction)
        {
            switch (directionType)
            {
                case BoneMotionDirectionType.Linear:
                    return direction;
                case BoneMotionDirectionType.NotBack:
                    if (direction.Degree < 180f) return SpriteDirection.FromDegree(360f - direction.Degree);
                    else return direction;
                case BoneMotionDirectionType.DownwardOnly:
                    return SpriteDirection.Down;
                default:
                    throw new System.ArgumentException();
            }
        }
    }
}
