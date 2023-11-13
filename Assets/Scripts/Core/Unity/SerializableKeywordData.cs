using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Keyword/Serializable")]
    [ObjectFormer.Referable]
    public class SerializableKeywordData : KeywordData, ISerializableKeyword
    {
    }
}
