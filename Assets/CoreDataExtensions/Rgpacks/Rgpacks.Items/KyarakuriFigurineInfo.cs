using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    public static class KyarakuriFigurineInfo
    {
        public static CharacterCreationDataBuilder Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.data;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// è„èëÇ´ïsâ¬
        /// </summary>
        public static void SetTo(RogueObj obj, CharacterCreationDataBuilder data)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            info.data = data;
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public CharacterCreationDataBuilder data;

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
