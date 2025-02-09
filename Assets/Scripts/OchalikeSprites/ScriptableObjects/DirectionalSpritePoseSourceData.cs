using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public abstract class DirectionalSpritePoseSourceData : ScriptableObject, IDirectionalSpritePoseSource
    {
        public abstract SpritePose GetSpritePose(SpriteDirection direction);
    }
}
