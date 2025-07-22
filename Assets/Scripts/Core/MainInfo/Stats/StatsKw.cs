using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class StatsKw : ScriptableLoader
    {
        private static StatsKw instance;

        [SerializeField] private KeywordAsset _atk;
        public static IKeyword Atk => instance._atk;

        [SerializeField] private KeywordAsset _cost;
        public static IKeyword Cost => instance._cost;

        [SerializeField] private KeywordAsset _critical;
        public static IKeyword Critical => instance._critical;

        [SerializeField] private KeywordAsset _criticalAtk;
        public static IKeyword CriticalAtk => instance._criticalAtk;

        [SerializeField] private KeywordAsset _criticalRate;
        public static IKeyword CriticalRate => instance._criticalRate;

        [SerializeField] private KeywordAsset _def;
        public static IKeyword Def => instance._def;

        [SerializeField] private KeywordAsset _exp;
        public static IKeyword Exp => instance._exp;

        [SerializeField] private KeywordAsset _female;
        public static IKeyword Female => instance._female;

        [SerializeField] private KeywordAsset _gender;
        public static IKeyword Gender => instance._gender;

        [SerializeField] private KeywordAsset _looksFemale;
        public static IKeyword LooksFemale => instance._looksFemale;

        [SerializeField] private KeywordAsset _looksMale;
        public static IKeyword LooksMale => instance._looksMale;

        [SerializeField] private KeywordAsset _male;
        public static IKeyword Male => instance._male;

        [SerializeField] private KeywordAsset _guaranteedDamage;
        public static IKeyword GuaranteedDamage => instance._guaranteedDamage;

        [SerializeField] private KeywordAsset _guard;
        public static IKeyword Guard => instance._guard;

        [SerializeField] private KeywordAsset _guardDef;
        public static IKeyword GuardDef => instance._guardDef;

        [SerializeField] private KeywordAsset _guardRate;
        public static IKeyword GuardRate => instance._guardRate;

        [SerializeField] private KeywordAsset _hp;
        public static IKeyword Hp => instance._hp;

        [SerializeField] private KeywordAsset _hpRegenerationPermille;
        public static IKeyword HpRegenerationPermille => instance._hpRegenerationPermille;

        [SerializeField] private KeywordAsset _loadCapacity;
        public static IKeyword LoadCapacity => instance._loadCapacity;

        [SerializeField] private KeywordAsset _material;
        public static IKeyword Material => instance._material;

        [SerializeField] private KeywordAsset _maxHp;
        public static IKeyword MaxHp => instance._maxHp;

        [SerializeField] private KeywordAsset _maxMp;
        public static IKeyword MaxMp => instance._maxMp;

        [SerializeField] private KeywordAsset _maxNutrition;
        public static IKeyword MaxNutrition => instance._maxNutrition;

        [SerializeField] private KeywordAsset _asStorage;
        public static IKeyword AsStorage => instance._asStorage;

        [SerializeField] private KeywordAsset _asTile;
        public static IKeyword AsTile => instance._asTile;

        [SerializeField] private KeywordAsset _hasCollider;
        public static IKeyword HasCollider => instance._hasCollider;

        [SerializeField] private KeywordAsset _hasSightCollider;
        public static IKeyword HasSightCollider => instance._hasSightCollider;

        [SerializeField] private KeywordAsset _hasTileCollider;
        public static IKeyword HasTileCollider => instance._hasTileCollider;

        [SerializeField] private KeywordAsset _movement;
        public static IKeyword Movement => instance._movement;

        [SerializeField] private KeywordAsset _mp;
        public static IKeyword Mp => instance._mp;

        [SerializeField] private KeywordAsset _mpRegenerationPermille;
        public static IKeyword MpRegenerationPermille => instance._mpRegenerationPermille;

        [SerializeField] private KeywordAsset _requiredMp;
        public static IKeyword RequiredMp => instance._requiredMp;

        [SerializeField] private KeywordAsset _spaceWeight;
        public static IKeyword SpaceWeight => instance._spaceWeight;

        [SerializeField] private KeywordAsset _beInhibited;
        public static IKeyword BeInhibited => instance._beInhibited;

        [SerializeField] private KeywordAsset _hungry;
        public static IKeyword Hungry => instance._hungry;

        [SerializeField] private KeywordAsset _speed;
        public static IKeyword Speed => instance._speed;

        [SerializeField] private KeywordAsset _weight;
        public static IKeyword Weight => instance._weight;

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
