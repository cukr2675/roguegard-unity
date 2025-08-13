using OchalikeSprites;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="ScriptableObject"/> を継承した <see cref="RogueSpriteMotion"/>
    /// </summary>
    public abstract class RogueSpriteMotionAsset : SpriteMotionAsset, IRogueSpriteMotion
    {
        public abstract IKeyword Keyword { get; }

        private static readonly MotionSet motionSet = new();

        public abstract void ApplyTo(
            ISpriteMotionSet motionSet, int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion);

        public override void ApplyTo(int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion)
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
