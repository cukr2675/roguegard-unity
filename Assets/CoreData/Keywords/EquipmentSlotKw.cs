using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class EquipmentSlotKw : ScriptableLoader
    {
        private static EquipmentSlotKw instance;

        [SerializeField] private EquipmentSlotKeywordAsset _accessory;
        public static ISerializableKeyword Accessory => instance._accessory;

        [SerializeField] private EquipmentSlotKeywordAsset _ammo;
        public static ISerializableKeyword Ammo => instance._ammo;

        [SerializeField] private EquipmentSlotKeywordAsset _bodyArmor;
        public static ISerializableKeyword BodyArmor => instance._bodyArmor;

        [SerializeField] private EquipmentSlotKeywordAsset _boots;
        public static ISerializableKeyword Boots => instance._boots;

        [SerializeField] private EquipmentSlotKeywordAsset _bottoms;
        public static ISerializableKeyword Bottoms => instance._bottoms;

        [SerializeField] private EquipmentSlotKeywordAsset _cloak;
        public static ISerializableKeyword Cloak => instance._cloak;

        [SerializeField] private EquipmentSlotKeywordAsset _faceMask;
        public static ISerializableKeyword FaceMask => instance._faceMask;

        [SerializeField] private EquipmentSlotKeywordAsset _glasses;
        public static ISerializableKeyword Glasses => instance._glasses;

        [SerializeField] private EquipmentSlotKeywordAsset _gloves;
        public static ISerializableKeyword Gloves => instance._gloves;

        [SerializeField] private EquipmentSlotKeywordAsset _headwear;
        public static ISerializableKeyword Headwear => instance._headwear;

        [SerializeField] private EquipmentSlotKeywordAsset _innerwear;
        public static ISerializableKeyword Innerwear => instance._innerwear;

        [SerializeField] private EquipmentSlotKeywordAsset _shield;
        public static ISerializableKeyword Shield => instance._shield;

        [SerializeField] private EquipmentSlotKeywordAsset _socks;
        public static ISerializableKeyword Socks => instance._socks;

        [SerializeField] private EquipmentSlotKeywordAsset _tops;
        public static ISerializableKeyword Tops => instance._tops;

        [SerializeField] private EquipmentSlotKeywordAsset _weapon;
        public static ISerializableKeyword Weapon => instance._weapon;

        public override IEnumerator LoadAsync()
        {
            instance = this;
            yield break;
        }

        public override void TestLoad()
        {
#if UNITY_EDITOR
            instance = this;
#else
            throw new System.InvalidOperationException("This method is Editor Only.");
#endif
        }
    }
}
