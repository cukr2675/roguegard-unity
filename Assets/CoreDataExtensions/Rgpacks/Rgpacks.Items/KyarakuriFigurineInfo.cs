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
        /// 上書き不可
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

            public bool CanStack(IRogueObjInfo coming) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
