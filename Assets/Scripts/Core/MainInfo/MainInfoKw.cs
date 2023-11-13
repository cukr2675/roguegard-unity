using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class MainInfoKw : ScriptableLoader
    {
        private static MainInfoKw instance;

        [SerializeField] private KeywordData _attack;
        public static IKeyword Attack => instance._attack;

        [SerializeField] private KeywordData _beApplied;
        public static IKeyword BeApplied => instance._beApplied;

        [SerializeField] private KeywordData _beDefeated;
        public static IKeyword BeDefeated => instance._beDefeated;

        [SerializeField] private KeywordData _beEaten;
        public static IKeyword BeEaten => instance._beEaten;

        [SerializeField] private KeywordData _beThrown;
        public static IKeyword BeThrown => instance._beThrown;

        [SerializeField] private KeywordData _eat;
        public static IKeyword Eat => instance._eat;

        [SerializeField] private KeywordData _equip;
        public static IKeyword Equip => instance._equip;

        [SerializeField] private KeywordData _hit;
        public static IKeyword Hit => instance._hit;

        [SerializeField] private KeywordData _locate;
        public static IKeyword Locate => instance._locate;

        [SerializeField] private KeywordData _pickUp;
        public static IKeyword PickUp => instance._pickUp;

        [SerializeField] private KeywordData _polymorph;
        public static IKeyword Polymorph => instance._polymorph;

        [SerializeField] private KeywordData _put;
        public static IKeyword Put => instance._put;

        [SerializeField] private KeywordData _skill;
        public static IKeyword Skill => instance._skill;

        [SerializeField] private KeywordData _throw;
        public static IKeyword Throw => instance._throw;

        [SerializeField] private KeywordData _unequip;
        public static IKeyword Unequip => instance._unequip;

        [SerializeField] private KeywordData _wait;
        public static IKeyword Wait => instance._wait;

        [SerializeField] private KeywordData _walk;
        public static IKeyword Walk => instance._walk;

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
