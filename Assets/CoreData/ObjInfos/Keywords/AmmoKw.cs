using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class AmmoKw : ScriptableLoader
    {
        private static AmmoKw instance;

        [SerializeField] private KeywordData _arrow;
        public static IKeyword Arrow => instance._arrow;

        [SerializeField] private KeywordData _bullet;
        public static IKeyword Bullet => instance._bullet;

        [SerializeField] private KeywordData _throwingKnife;
        public static IKeyword ThrowingKnife => instance._throwingKnife;

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
