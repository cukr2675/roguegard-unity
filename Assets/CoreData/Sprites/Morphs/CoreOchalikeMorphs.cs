using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CoreOchalikeMorphs : ScriptableLoader
    {
        private static CoreOchalikeMorphs instance;

        [SerializeField] private OchalikeSprites.OchalikeMorphData _guruguruEyes;
        public static OchalikeSprites.OchalikeMorphData GuruguruEyes => instance._guruguruEyes;

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
