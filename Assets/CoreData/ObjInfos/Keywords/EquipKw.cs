using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class EquipKw : ScriptableLoader
    {
        private static EquipKw instance;

        [SerializeField] private KeywordData _accessory;
        public static IKeyword Accessory => instance._accessory;

        [SerializeField] private KeywordData _ammo;
        public static IKeyword Ammo => instance._ammo;

        [SerializeField] private KeywordData _bodyArmor;
        public static IKeyword BodyArmor => instance._bodyArmor;

        [SerializeField] private KeywordData _boots;
        public static IKeyword Boots => instance._boots;

        [SerializeField] private KeywordData _bottoms;
        public static IKeyword Bottoms => instance._bottoms;

        [SerializeField] private KeywordData _cloak;
        public static IKeyword Cloak => instance._cloak;

        [SerializeField] private KeywordData _faceMask;
        public static IKeyword FaceMask => instance._faceMask;

        [SerializeField] private KeywordData _gloves;
        public static IKeyword Gloves => instance._gloves;

        [SerializeField] private KeywordData _headwear;
        public static IKeyword Headwear => instance._headwear;

        [SerializeField] private KeywordData _innerwear;
        public static IKeyword Innerwear => instance._innerwear;

        [SerializeField] private KeywordData _lenses;
        public static IKeyword Lenses => instance._lenses;

        [SerializeField] private KeywordData _shield;
        public static IKeyword Shield => instance._shield;

        [SerializeField] private KeywordData _socks;
        public static IKeyword Socks => instance._socks;

        [SerializeField] private KeywordData _tops;
        public static IKeyword Tops => instance._tops;

        [SerializeField] private KeywordData _weapon;
        public static IKeyword Weapon => instance._weapon;

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
