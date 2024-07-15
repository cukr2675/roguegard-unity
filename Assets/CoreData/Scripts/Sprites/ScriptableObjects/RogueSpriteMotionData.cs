using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;

namespace Roguegard
{
    public abstract class RogueSpriteMotionData : SpriteMotionData, IRogueSpriteMotion
    {
        public abstract IKeyword Keyword { get; }

        private static readonly MotionSet motionSet = new MotionSet();

        public abstract void ApplyTo(
            ISpriteMotionSet motionSet, int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion);

        public override void ApplyTo(int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion)
        {
            ApplyTo(motionSet, animationTime, direction, ref transform, out endOfMotion);
        }

        private class MotionSet : ISpriteMotionSet
        {
            public void GetPose(IKeyword keyword, int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion)
            {
                endOfMotion = true;
            }
        }
    }
}
