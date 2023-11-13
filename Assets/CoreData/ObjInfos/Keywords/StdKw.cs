using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class StdKw : ScriptableLoader
    {
        private static StdKw instance;

        [SerializeField] private KeywordData _apply;
        public static IKeyword Apply => instance._apply;

        [SerializeField] private KeywordData _beShot;
        public static IKeyword BeShot => instance._beShot;

        [SerializeField] private KeywordData _heal;
        public static IKeyword Heal => instance._heal;

        [SerializeField] private KeywordData _loseExp;
        public static IKeyword LoseExp => instance._loseExp;

        [SerializeField] private KeywordData _fear;
        public static IKeyword Fear => instance._fear;

        [SerializeField] private KeywordData _levitation;
        public static IKeyword Levitation => instance._levitation;

        [SerializeField] private KeywordData _poolMovement;
        public static IKeyword PoolMovement => instance._poolMovement;

        [SerializeField] private KeywordData _push;
        public static IKeyword Push => instance._push;

        [SerializeField] private KeywordData _putIntoChest;
        public static IKeyword PutIntoChest => instance._putIntoChest;

        [SerializeField] private KeywordData _read;
        public static IKeyword Read => instance._read;

        [SerializeField] private KeywordData _ride;
        public static IKeyword Ride => instance._ride;

        [SerializeField] private KeywordData _bomb;
        public static IKeyword Bomb => instance._bomb;

        [SerializeField] private KeywordData _confusion;
        public static IKeyword Confusion => instance._confusion;

        [SerializeField] private KeywordData _gunThrow;
        public static IKeyword GunThrow => instance._gunThrow;

        [SerializeField] private KeywordData _levelDown;
        public static IKeyword LevelDown => instance._levelDown;

        [SerializeField] private KeywordData _levelUp;
        public static IKeyword LevelUp => instance._levelUp;

        [SerializeField] private KeywordData _noDamage;
        public static IKeyword NoDamage => instance._noDamage;

        [SerializeField] private KeywordData _paralysis;
        public static IKeyword Paralysis => instance._paralysis;

        [SerializeField] private KeywordData _poison;
        public static IKeyword Poison => instance._poison;

        [SerializeField] private KeywordData _powerSlash;
        public static IKeyword PowerSlash => instance._powerSlash;

        [SerializeField] private KeywordData _pyro;
        public static IKeyword Pyro => instance._pyro;

        [SerializeField] private KeywordData _sort;
        public static IKeyword Sort => instance._sort;

        [SerializeField] private KeywordData _statusEffect;
        public static IKeyword StatusEffect => instance._statusEffect;

        [SerializeField] private KeywordData _teleport;
        public static IKeyword Teleport => instance._teleport;

        [SerializeField] private KeywordData _digestion;
        public static IKeyword Digestion => instance._digestion;

        [SerializeField] private KeywordData _stepOn;
        public static IKeyword StepOn => instance._stepOn;

        [SerializeField] private KeywordData _takeOutFromChest;
        public static IKeyword TakeOutFromChest => instance._takeOutFromChest;

        [SerializeField] private KeywordData _turn;
        public static IKeyword Turn => instance._turn;

        [SerializeField] private KeywordData _unride;
        public static IKeyword Unride => instance._unride;

        [SerializeField] private KeywordData _vehicle;
        public static IKeyword Vehicle => instance._vehicle;

        [SerializeField] private KeywordData _victory;
        public static IKeyword Victory => instance._victory;

        [SerializeField] private KeywordData _view;
        public static IKeyword View => instance._view;

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
