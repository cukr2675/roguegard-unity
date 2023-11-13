using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CharacterCreationKw : ScriptableLoader
    {
        private static CharacterCreationKw instance;

        [SerializeField] private KeywordData _alpha;
        public static IKeyword Alpha => instance._alpha;

        [SerializeField] private KeywordData _blue;
        public static IKeyword Blue => instance._blue;

        [SerializeField] private KeywordData _color;
        public static IKeyword Color => instance._color;

        [SerializeField] private KeywordData _glued;
        public static IKeyword Glued => instance._glued;

        [SerializeField] private KeywordData _green;
        public static IKeyword Green => instance._green;

        [SerializeField] private KeywordData _red;
        public static IKeyword Red => instance._red;

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
