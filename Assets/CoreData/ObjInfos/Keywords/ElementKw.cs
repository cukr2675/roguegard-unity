using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class ElementKw : ScriptableLoader
    {
        private static ElementKw instance;

        [SerializeField] private KeywordAsset _fire;
        public static IKeyword Fire => instance._fire;

        [SerializeField] private KeywordAsset _ice;
        public static IKeyword Ice => instance._ice;

        [SerializeField] private KeywordAsset _thunder;
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
            throw new System.InvalidOperationException("This method is Editor Only.");
#endif
        }
    }
}
