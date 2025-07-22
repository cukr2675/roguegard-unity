using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class CategoryKw : ScriptableLoader
    {
        private static CategoryKw instance;

        [SerializeField] private KeywordAsset _applyTool;
        public static IKeyword ApplyTool => instance._applyTool;

        [SerializeField] private KeywordAsset _container;
        public static IKeyword Container => instance._container;

        [SerializeField] private KeywordAsset _downStairs;
        public static IKeyword DownStairs => instance._downStairs;

        [SerializeField] private KeywordAsset _drink;
        public static IKeyword Drink => instance._drink;

        [SerializeField] private KeywordAsset _equipment;
        public static IKeyword Equipment => instance._equipment;

        [SerializeField] private KeywordAsset _food;
        public static IKeyword Food => instance._food;

        [SerializeField] private KeywordAsset _levelDownStairs;
        public static IKeyword LevelDownStairs => instance._levelDownStairs;

        [SerializeField] private KeywordAsset _movableObstacle;
        public static IKeyword MovableObstacle => instance._movableObstacle;

        [SerializeField] private KeywordAsset _pool;
        public static IKeyword Pool => instance._pool;

        [SerializeField] private KeywordAsset _readable;
        public static IKeyword Readable => instance._readable;

        [SerializeField] private KeywordAsset _trap;
        public static IKeyword Trap => instance._trap;

        [SerializeField] private KeywordAsset _vehicle;
        public static IKeyword Vehicle => instance._vehicle;

        [SerializeField] private KeywordAsset _wand;
        public static IKeyword Wand => instance._wand;

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
