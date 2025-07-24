using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class MainInfoKw : ScriptableLoader
    {
        private static MainInfoKw instance;

        [SerializeField] private KeywordAsset _attack;
        public static IKeyword Attack => instance._attack;

        [SerializeField] private KeywordAsset _beApplied;
        public static IKeyword BeApplied => instance._beApplied;

        [SerializeField] private KeywordAsset _beDefeated;
        public static IKeyword BeDefeated => instance._beDefeated;

        [SerializeField] private KeywordAsset _beEaten;
        public static IKeyword BeEaten => instance._beEaten;

        [SerializeField] private KeywordAsset _beThrown;
        public static IKeyword BeThrown => instance._beThrown;

        [SerializeField] private KeywordAsset _eat;
        public static IKeyword Eat => instance._eat;

        [SerializeField] private KeywordAsset _equip;
        public static IKeyword Equip => instance._equip;

        [SerializeField] private KeywordAsset _hit;
        public static IKeyword Hit => instance._hit;

        [SerializeField] private KeywordAsset _locate;
        public static IKeyword Locate => instance._locate;

        [SerializeField] private KeywordAsset _pickUp;
        public static IKeyword PickUp => instance._pickUp;

        [SerializeField] private KeywordAsset _polymorph;
        public static IKeyword Polymorph => instance._polymorph;

        [SerializeField] private KeywordAsset _put;
        public static IKeyword Put => instance._put;

        [SerializeField] private KeywordAsset _skill;
        public static IKeyword Skill => instance._skill;

        [SerializeField] private KeywordAsset _throw;
        public static IKeyword Throw => instance._throw;

        [SerializeField] private KeywordAsset _unequip;
        public static IKeyword Unequip => instance._unequip;

        [SerializeField] private KeywordAsset _wait;
        public static IKeyword Wait => instance._wait;

        [SerializeField] private KeywordAsset _walk;
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
            throw new System.InvalidOperationException("This method is Editor Only.");
#endif
        }
    }
}
