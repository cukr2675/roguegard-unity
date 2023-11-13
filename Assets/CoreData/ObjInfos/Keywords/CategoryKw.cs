using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CategoryKw : ScriptableLoader
    {
        private static CategoryKw instance;

        [SerializeField] private KeywordData _applyTool;
        public static IKeyword ApplyTool => instance._applyTool;

        [SerializeField] private KeywordData _chest;
        public static IKeyword Chest => instance._chest;

        [SerializeField] private KeywordData _downStairs;
        public static IKeyword DownStairs => instance._downStairs;

        [SerializeField] private KeywordData _drink;
        public static IKeyword Drink => instance._drink;

        [SerializeField] private KeywordData _equipment;
        public static IKeyword Equipment => instance._equipment;

        [SerializeField] private KeywordData _food;
        public static IKeyword Food => instance._food;

        [SerializeField] private KeywordData _levelDownStairs;
        public static IKeyword LevelDownStairs => instance._levelDownStairs;

        [SerializeField] private KeywordData _movableObstacle;
        public static IKeyword MovableObstacle => instance._movableObstacle;

        [SerializeField] private KeywordData _pool;
        public static IKeyword Pool => instance._pool;

        [SerializeField] private KeywordData _readable;
        public static IKeyword Readable => instance._readable;

        [SerializeField] private KeywordData _trap;
        public static IKeyword Trap => instance._trap;

        [SerializeField] private KeywordData _vehicle;
        public static IKeyword Vehicle => instance._vehicle;

        [SerializeField] private KeywordData _wand;
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
