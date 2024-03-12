using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public abstract class DirectionalBonePoseSourceData : ScriptableObject, IDirectionalBonePoseSource
    {
        public abstract BonePose GetBonePose(SpriteDirection direction);
    }
}
