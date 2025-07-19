using OchalikeSprites;

namespace Roguegard
{
    public interface IRogueSpriteMotion : ISpriteMotion
    {
        IKeyword Keyword { get; }

        void ApplyTo(ISpriteMotionSet motionSet, int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion);
    }
}
