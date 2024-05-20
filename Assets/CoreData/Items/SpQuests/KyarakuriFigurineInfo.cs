using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    [Objforming.Formable]
    public class KyarakuriFigurineInfo
    {
        public string ID { get; set; }
        public CharacterCreationDataBuilder Main { get; set; }

        private KyarakuriFigurineInfo() { }

        public static KyarakuriFigurineInfo Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// è„èëÇ´ïsâ¬
        /// </summary>
        public static void SetTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            // è„èëÇ´ïsâ¬
            if (info.info != null) throw new RogueException();

            info.info = new KyarakuriFigurineInfo();
            info.info.Main = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public KyarakuriFigurineInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other)
            {
                return false;
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return null;
            }

            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }
    }
}
