using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class StatsKw : ScriptableLoader
    {
        private static StatsKw instance;

        [SerializeField] private KeywordData _atk;
        public static IKeyword ATK => instance._atk;

        [SerializeField] private KeywordData _cost;
        public static IKeyword Cost => instance._cost;

        [SerializeField] private KeywordData _critical;
        public static IKeyword Critical => instance._critical;

        [SerializeField] private KeywordData _criticalATK;
        public static IKeyword CriticalATK => instance._criticalATK;

        [SerializeField] private KeywordData _criticalRate;
        public static IKeyword CriticalRate => instance._criticalRate;

        [SerializeField] private KeywordData _def;
        public static IKeyword DEF => instance._def;

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

        [SerializeField] private KeywordData _guardDEF;
        public static IKeyword GuardDEF => instance._guardDEF;

        [SerializeField] private KeywordData _guardRate;
        public static IKeyword GuardRate => instance._guardRate;

        [SerializeField] private KeywordData _hp;
        public static IKeyword HP => instance._hp;

        [SerializeField] private KeywordData _hpregenerationPermille;
        public static IKeyword HPRegenerationPermille => instance._hpregenerationPermille;

        [SerializeField] private KeywordData _loadCapacity;
        public static IKeyword LoadCapacity => instance._loadCapacity;

        [SerializeField] private KeywordData _material;
        public static IKeyword Material => instance._material;

        [SerializeField] private KeywordData _maxHP;
        public static IKeyword MaxHP => instance._maxHP;

        [SerializeField] private KeywordData _maxMP;
        public static IKeyword MaxMP => instance._maxMP;

        [SerializeField] private KeywordData _maxNutrition;
        public static IKeyword MaxNutrition => instance._maxNutrition;

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
        public static IKeyword MP => instance._mp;

        [SerializeField] private KeywordData _mpregenerationPermille;
        public static IKeyword MPRegenerationPermille => instance._mpregenerationPermille;

        [SerializeField] private KeywordData _requiredMP;
        public static IKeyword RequiredMP => instance._requiredMP;

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
