using UnityEngine;

namespace OchalikeSprites
{
    [CreateAssetMenu(menuName = "Ochalike Sprites/Bone Keyword")]
    public class BoneKeywordAsset : ScriptableObject
    {
        public static implicit operator BoneKeyword(BoneKeywordAsset data)
        {
            return new BoneKeyword(data.name);
        }
    }
}
