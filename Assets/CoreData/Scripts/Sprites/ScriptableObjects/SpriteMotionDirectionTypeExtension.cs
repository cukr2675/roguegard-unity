using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public static class SpriteMotionDirectionTypeExtension
    {
        public static SpriteDirection Convert(this SpriteMotionDirectionType directionType, SpriteDirection direction)
        {
            switch (directionType)
            {
                case SpriteMotionDirectionType.Linear:
                    return direction;
                case SpriteMotionDirectionType.NotBack:
                    if (direction.Degree < 180f) return SpriteDirection.FromDegree(360f - direction.Degree);
                    else return direction;
                case SpriteMotionDirectionType.DownwardOnly:
                    return SpriteDirection.Down;
                default:
                    throw new System.ArgumentException();
            }
        }
    }
}
