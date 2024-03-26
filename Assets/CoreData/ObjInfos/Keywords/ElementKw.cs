using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class ElementKw : ScriptableLoader
    {
        private static ElementKw instance;

        [SerializeField] private KeywordData _fire;
        public static IKeyword Fire => instance._fire;

        [SerializeField] private KeywordData _ice;
        public static IKeyword Ice => instance._ice;

        [SerializeField] private KeywordData _thunder;
        public static IKeyword Thunder => instance._thunder;

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
