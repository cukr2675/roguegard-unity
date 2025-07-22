using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class MaterialKw : ScriptableLoader
    {
        private static MaterialKw instance;

        [SerializeField] private KeywordAsset _flammable;
        public static IKeyword Flammable => instance._flammable;

        [SerializeField] private KeywordAsset _flesh;
        public static IKeyword Flesh => instance._flesh;

        [SerializeField] private KeywordAsset _iron;
        public static IKeyword Iron => instance._iron;

        [SerializeField] private KeywordAsset _metallic;
        public static IKeyword Metallic => instance._metallic;

        [SerializeField] private KeywordAsset _organic;
        public static IKeyword Organic => instance._organic;

        [SerializeField] private KeywordAsset _veggy;
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
