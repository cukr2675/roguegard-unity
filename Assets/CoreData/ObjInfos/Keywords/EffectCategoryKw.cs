using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class EffectCategoryKw : ScriptableLoader
    {
        private static EffectCategoryKw instance;

        [SerializeField] private KeywordAsset _buff;
        public static IKeyword Buff => instance._buff;

        [SerializeField] private KeywordAsset _debuff;
        public static IKeyword Debuff => instance._debuff;

        [SerializeField] private KeywordAsset _dummy;
        public static IKeyword Dummy => instance._dummy;

        [SerializeField] private KeywordAsset _erosion;
        public static IKeyword Erosion => instance._erosion;

        [SerializeField] private KeywordAsset _statusAilment;
        public static IKeyword StatusAilment => instance._statusAilment;

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
