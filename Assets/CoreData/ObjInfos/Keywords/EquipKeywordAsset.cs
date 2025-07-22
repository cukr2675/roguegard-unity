using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "Roguegard/Keyword/Equip")]
    [Objforming.Referable]
    public class EquipKeywordAsset : SerializableKeywordAsset
    {
        [Header("Equip")]
        [SerializeField] private float _order = 0f;
        public float Order => _order;
    }
}
