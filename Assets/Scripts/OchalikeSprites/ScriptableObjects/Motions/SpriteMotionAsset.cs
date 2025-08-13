using UnityEngine;

namespace OchalikeSprites
{
    public abstract class SpriteMotionAsset : ScriptableObject, ISpriteMotion
    {
        public abstract void ApplyTo(int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion);
    }
}
