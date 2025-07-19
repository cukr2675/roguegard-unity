using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "Roguegard/Keyword/Serializable")]
    [Objforming.Referable]
    public class SerializableKeywordData : KeywordData, ISerializableKeyword
    {
    }
}
