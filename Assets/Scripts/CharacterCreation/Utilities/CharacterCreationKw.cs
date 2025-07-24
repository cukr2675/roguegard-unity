using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class CharacterCreationKw : ScriptableLoader
    {
        private static CharacterCreationKw instance;

        [SerializeField] private KeywordAsset _alpha;
        public static IKeyword Alpha => instance._alpha;

        [SerializeField] private KeywordAsset _blue;
        public static IKeyword Blue => instance._blue;

        [SerializeField] private KeywordAsset _color;
        public static IKeyword Color => instance._color;

        [SerializeField] private KeywordAsset _glued;
        public static IKeyword Glued => instance._glued;

        [SerializeField] private KeywordAsset _green;
        public static IKeyword Green => instance._green;

        [SerializeField] private KeywordAsset _red;
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
            throw new System.InvalidOperationException("This method is Editor Only.");
#endif
        }
    }
}
