using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;

namespace Roguegard
{
    /// <summary>
    /// 関連: <see cref="RogueSpriteMotionData"/>
    /// </summary>
    public abstract class RogueSpriteMotion : IRogueSpriteMotion
    {
        public abstract IKeyword Keyword { get; }

        private static readonly MotionSet motionSet = new();

        public abstract void ApplyTo(
            ISpriteMotionSet motionSet, int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion);

        public void ApplyTo(int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion)
        {
            ApplyTo(motionSet, animationTime, direction, ref transform, out endOfMotion);
        }

        private class MotionSet : ISpriteMotionSet
        {
            public void GetPose(IKeyword keyword, int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion)
            {
                endOfMotion = true;
            }
        }
    }
}
