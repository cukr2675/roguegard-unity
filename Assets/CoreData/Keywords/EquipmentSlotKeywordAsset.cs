using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "Roguegard/Keyword/Equipment Slot")]
    [Objforming.Referable]
    public class EquipmentSlotKeywordAsset : SerializableKeywordAsset
    {
        [Header("Equip")]
        [SerializeField] private float _order = 0f;
        public float Order => _order;
    }
}
