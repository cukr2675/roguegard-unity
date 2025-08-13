using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class EquipKw : ScriptableLoader
    {
        private static EquipKw instance;

        [SerializeField] private EquipKeywordAsset _accessory;
        public static ISerializableKeyword Accessory => instance._accessory;

        [SerializeField] private EquipKeywordAsset _ammo;
        public static ISerializableKeyword Ammo => instance._ammo;

        [SerializeField] private EquipKeywordAsset _bodyArmor;
        public static ISerializableKeyword BodyArmor => instance._bodyArmor;

        [SerializeField] private EquipKeywordAsset _boots;
        public static ISerializableKeyword Boots => instance._boots;

        [SerializeField] private EquipKeywordAsset _bottoms;
        public static ISerializableKeyword Bottoms => instance._bottoms;

        [SerializeField] private EquipKeywordAsset _cloak;
        public static ISerializableKeyword Cloak => instance._cloak;

        [SerializeField] private EquipKeywordAsset _faceMask;
        public static ISerializableKeyword FaceMask => instance._faceMask;

        [SerializeField] private EquipKeywordAsset _glasses;
        public static ISerializableKeyword Glasses => instance._glasses;

        [SerializeField] private EquipKeywordAsset _gloves;
        public static ISerializableKeyword Gloves => instance._gloves;

        [SerializeField] private EquipKeywordAsset _headwear;
        public static ISerializableKeyword Headwear => instance._headwear;

        [SerializeField] private EquipKeywordAsset _innerwear;
        public static ISerializableKeyword Innerwear => instance._innerwear;

        [SerializeField] private EquipKeywordAsset _shield;
        public static ISerializableKeyword Shield => instance._shield;

        [SerializeField] private EquipKeywordAsset _socks;
        public static ISerializableKeyword Socks => instance._socks;

        [SerializeField] private EquipKeywordAsset _tops;
        public static ISerializableKeyword Tops => instance._tops;

        [SerializeField] private EquipKeywordAsset _weapon;
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
