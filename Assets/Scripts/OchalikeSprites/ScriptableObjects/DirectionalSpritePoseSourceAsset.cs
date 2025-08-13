using UnityEngine;

namespace OchalikeSprites
{
    public abstract class DirectionalSpritePoseSourceAsset : ScriptableObject, IDirectionalSpritePoseSource
    {
        public abstract SpritePose GetSpritePose(SpriteDirection direction);
    }
}
