using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class StatsKw : ScriptableLoader
    {
        private static StatsKw instance;

        [SerializeField] private KeywordData _atk;
        public static IKeyword Atk => instance._atk;

        [SerializeField] private KeywordData _cost;
        public static IKeyword Cost => instance._cost;

        [SerializeField] private KeywordData _critical;
        public static IKeyword Critical => instance._critical;

        [SerializeField] private KeywordData _criticalAtk;
        public static IKeyword CriticalAtk => instance._criticalAtk;

        [SerializeField] private KeywordData _criticalRate;
        public static IKeyword CriticalRate => instance._criticalRate;

        [SerializeField] private KeywordData _def;
        public static IKeyword Def => instance._def;

        [SerializeField] private KeywordData _exp;
        public static IKeyword Exp => instance._exp;

        [SerializeField] private KeywordData _female;
        public static IKeyword Female => instance._female;

        [SerializeField] private KeywordData _gender;
        public static IKeyword Gender => instance._gender;

        [SerializeField] private KeywordData _looksFemale;
        public static IKeyword LooksFemale => instance._looksFemale;

        [SerializeField] private KeywordData _looksMale;
        public static IKeyword LooksMale => instance._looksMale;

        [SerializeField] private KeywordData _male;
        public static IKeyword Male => instance._male;

        [SerializeField] private KeywordData _guaranteedDamage;
        public static IKeyword GuaranteedDamage => instance._guaranteedDamage;

        [SerializeField] private KeywordData _guard;
        public static IKeyword Guard => instance._guard;

        [SerializeField] private KeywordData _guardDef;
        public static IKeyword GuardDef => instance._guardDef;

        [SerializeField] private KeywordData _guardRate;
        public static IKeyword GuardRate => instance._guardRate;

        [SerializeField] private KeywordData _hp;
        public static IKeyword Hp => instance._hp;

        [SerializeField] private KeywordData _hpRegenerationPermille;
        public static IKeyword HpRegenerationPermille => instance._hpRegenerationPermille;

        [SerializeField] private KeywordData _loadCapacity;
        public static IKeyword LoadCapacity => instance._loadCapacity;

        [SerializeField] private KeywordData _material;
        public static IKeyword Material => instance._material;

        [SerializeField] private KeywordData _maxHp;
        public static IKeyword MaxHp => instance._maxHp;

        [SerializeField] private KeywordData _maxMp;
        public static IKeyword MaxMp => instance._maxMp;

        [SerializeField] private KeywordData _maxNutrition;
        public static IKeyword MaxNutrition => instance._maxNutrition;

        [SerializeField] private KeywordData _asStorage;
        public static IKeyword AsStorage => instance._asStorage;

        [SerializeField] private KeywordData _asTile;
        public static IKeyword AsTile => instance._asTile;

        [SerializeField] private KeywordData _hasCollider;
        public static IKeyword HasCollider => instance._hasCollider;

        [SerializeField] private KeywordData _hasSightCollider;
        public static IKeyword HasSightCollider => instance._hasSightCollider;

        [SerializeField] private KeywordData _hasTileCollider;
        public static IKeyword HasTileCollider => instance._hasTileCollider;

        [SerializeField] private KeywordData _movement;
        public static IKeyword Movement => instance._movement;

        [SerializeField] private KeywordData _mp;
        public static IKeyword Mp => instance._mp;

        [SerializeField] private KeywordData _mpRegenerationPermille;
        public static IKeyword MpRegenerationPermille => instance._mpRegenerationPermille;

        [SerializeField] private KeywordData _requiredMp;
        public static IKeyword RequiredMp => instance._requiredMp;

        [SerializeField] private KeywordData _spaceWeight;
        public static IKeyword SpaceWeight => instance._spaceWeight;

        [SerializeField] private KeywordData _beInhibited;
        public static IKeyword BeInhibited => instance._beInhibited;

        [SerializeField] private KeywordData _hungry;
        public static IKeyword Hungry => instance._hungry;

        [SerializeField] private KeywordData _speed;
        public static IKeyword Speed => instance._speed;

        [SerializeField] private KeywordData _weight;
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
