using UnityEngine;

namespace OchalikeSprites
{
    [CreateAssetMenu(menuName = "Ochalike Sprites/Bone Keyword")]
    public class BoneKeywordData : ScriptableObject
    {
        public static implicit operator BoneKeyword(BoneKeywordData data)
        {
            return new BoneKeyword(data.name);
        }
    }
}
