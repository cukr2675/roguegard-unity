using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CoreBoneSpriteTables : ScriptableLoader
    {
        private static CoreBoneSpriteTables instance;

        [SerializeField] private EffectableBoneSpriteTableData _guruguruEyes;
        public static EffectableBoneSpriteTableData GuruguruEyes => instance._guruguruEyes;

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
