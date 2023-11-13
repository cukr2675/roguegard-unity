using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class MaterialKw : ScriptableLoader
    {
        private static MaterialKw instance;

        [SerializeField] private KeywordData _flammable;
        public static IKeyword Flammable => instance._flammable;

        [SerializeField] private KeywordData _flesh;
        public static IKeyword Flesh => instance._flesh;

        [SerializeField] private KeywordData _iron;
        public static IKeyword Iron => instance._iron;

        [SerializeField] private KeywordData _metallic;
        public static IKeyword Metallic => instance._metallic;

        [SerializeField] private KeywordData _organic;
        public static IKeyword Organic => instance._organic;

        [SerializeField] private KeywordData _veggy;
        public static IKeyword Veggy => instance._veggy;

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
