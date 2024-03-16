using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public abstract class DirectionalSpritePoseSourceData : ScriptableObject, IDirectionalSpritePoseSource
    {
        public abstract SpritePose GetSpritePose(SpriteDirection direction);
    }
}
