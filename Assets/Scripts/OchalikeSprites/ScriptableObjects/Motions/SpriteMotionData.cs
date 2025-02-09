using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public abstract class SpriteMotionData : ScriptableObject, ISpriteMotion
    {
        public abstract void ApplyTo(int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion);
    }
}
