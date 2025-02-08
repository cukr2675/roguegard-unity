using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprite
{
    [CreateAssetMenu(menuName = "OchalikeSprite/BoneKeyword")]
    public class BoneKeywordData : ScriptableObject
    {
        public static implicit operator BoneKeyword(BoneKeywordData data)
        {
            return new BoneKeyword(data.name);
        }
    }
}
