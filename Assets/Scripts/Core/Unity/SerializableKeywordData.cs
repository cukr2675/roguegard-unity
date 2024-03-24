using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Keyword/Serializable")]
    [Objforming.Referable]
    public class SerializableKeywordData : KeywordData, ISerializableKeyword
    {
    }
}
