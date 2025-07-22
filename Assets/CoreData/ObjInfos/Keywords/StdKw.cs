using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class StdKw : ScriptableLoader
    {
        private static StdKw instance;

        [SerializeField] private KeywordAsset _apply;
        public static IKeyword Apply => instance._apply;

        [SerializeField] private KeywordAsset _beEntered;
        public static IKeyword BeEntered => instance._beEntered;

        [SerializeField] private KeywordAsset _beShot;
        public static IKeyword BeShot => instance._beShot;

        [SerializeField] private KeywordAsset _heal;
        public static IKeyword Heal => instance._heal;

        [SerializeField] private KeywordAsset _loseExp;
        public static IKeyword LoseExp => instance._loseExp;

        [SerializeField] private KeywordAsset _fear;
        public static IKeyword Fear => instance._fear;

        [SerializeField] private KeywordAsset _levitation;
        public static IKeyword Levitation => instance._levitation;

        [SerializeField] private KeywordAsset _poolMovement;
        public static IKeyword PoolMovement => instance._poolMovement;

        [SerializeField] private KeywordAsset _push;
        public static IKeyword Push => instance._push;

        [SerializeField] private KeywordAsset _putIntoContainer;
        public static IKeyword PutIntoContainer => instance._putIntoContainer;

        [SerializeField] private KeywordAsset _read;
        public static IKeyword Read => instance._read;

        [SerializeField] private KeywordAsset _ride;
        public static IKeyword Ride => instance._ride;

        [SerializeField] private KeywordAsset _bomb;
        public static IKeyword Bomb => instance._bomb;

        [SerializeField] private KeywordAsset _confusion;
        public static IKeyword Confusion => instance._confusion;

        [SerializeField] private KeywordAsset _gunThrow;
        public static IKeyword GunThrow => instance._gunThrow;

        [SerializeField] private KeywordAsset _levelDown;
        public static IKeyword LevelDown => instance._levelDown;

        [SerializeField] private KeywordAsset _levelUp;
        public static IKeyword LevelUp => instance._levelUp;

        [SerializeField] private KeywordAsset _noDamage;
        public static IKeyword NoDamage => instance._noDamage;

        [SerializeField] private KeywordAsset _paralysis;
        public static IKeyword Paralysis => instance._paralysis;

        [SerializeField] private KeywordAsset _poison;
        public static IKeyword Poison => instance._poison;

        [SerializeField] private KeywordAsset _powerSlash;
        public static IKeyword PowerSlash => instance._powerSlash;

        [SerializeField] private KeywordAsset _pyro;
        public static IKeyword Pyro => instance._pyro;

        [SerializeField] private KeywordAsset _sort;
        public static IKeyword Sort => instance._sort;

        [SerializeField] private KeywordAsset _statusEffect;
        public static IKeyword StatusEffect => instance._statusEffect;

        [SerializeField] private KeywordAsset _teleport;
        public static IKeyword Teleport => instance._teleport;

        [SerializeField] private KeywordAsset _digestion;
        public static IKeyword Digestion => instance._digestion;

        [SerializeField] private KeywordAsset _stepOn;
        public static IKeyword StepOn => instance._stepOn;

        [SerializeField] private KeywordAsset _takeOutOfContainer;
        public static IKeyword TakeOutOfContainer => instance._takeOutOfContainer;

        [SerializeField] private KeywordAsset _turn;
        public static IKeyword Turn => instance._turn;

        [SerializeField] private KeywordAsset _unride;
        public static IKeyword Unride => instance._unride;

        [SerializeField] private KeywordAsset _vehicle;
        public static IKeyword Vehicle => instance._vehicle;

        [SerializeField] private KeywordAsset _victory;
        public static IKeyword Victory => instance._victory;

        [SerializeField] private KeywordAsset _view;
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
