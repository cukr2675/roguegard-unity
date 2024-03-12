using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    [CreateAssetMenu(menuName = "SkeletalSprite/BoneKeyword")]
    public class BoneKeywordData : ScriptableObject
    {
        [SerializeField] private string _name = null;

        public static implicit operator BoneKeyword(BoneKeywordData data)
        {
            return new BoneKeyword(data._name);
        }
    }
}
