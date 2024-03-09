using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class EquipKw : ScriptableLoader
    {
        private static EquipKw instance;

        [SerializeField] private EquipKeywordData _accessory;
        public static ISerializableKeyword Accessory => instance._accessory;

        [SerializeField] private EquipKeywordData _ammo;
        public static ISerializableKeyword Ammo => instance._ammo;

        [SerializeField] private EquipKeywordData _bodyArmor;
        public static ISerializableKeyword BodyArmor => instance._bodyArmor;

        [SerializeField] private EquipKeywordData _boots;
        public static ISerializableKeyword Boots => instance._boots;

        [SerializeField] private EquipKeywordData _bottoms;
        public static ISerializableKeyword Bottoms => instance._bottoms;

        [SerializeField] private EquipKeywordData _cloak;
        public static ISerializableKeyword Cloak => instance._cloak;

        [SerializeField] private EquipKeywordData _faceMask;
        public static ISerializableKeyword FaceMask => instance._faceMask;

        [SerializeField] private EquipKeywordData _gloves;
        public static ISerializableKeyword Gloves => instance._gloves;

        [SerializeField] private EquipKeywordData _headwear;
        public static ISerializableKeyword Headwear => instance._headwear;

        [SerializeField] private EquipKeywordData _innerwear;
        public static ISerializableKeyword Innerwear => instance._innerwear;

        [SerializeField] private EquipKeywordData _lenses;
        public static ISerializableKeyword Lenses => instance._lenses;

        [SerializeField] private EquipKeywordData _shield;
        public static ISerializableKeyword Shield => instance._shield;

        [SerializeField] private EquipKeywordData _socks;
        public static ISerializableKeyword Socks => instance._socks;

        [SerializeField] private EquipKeywordData _tops;
        public static ISerializableKeyword Tops => instance._tops;

        [SerializeField] private EquipKeywordData _weapon;
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
            throw new RogueException("This method is Editor Only.");
#endif
        }
    }
}
