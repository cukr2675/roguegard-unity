using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class DioramaFloorInfo
    {
        public static IDioramaFloor Get(RogueObj obj)
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
        /// 上書き不可
        /// </summary>
        public static void SetTo(RogueObj obj, IDioramaFloor floor)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            // 上書き不可
            if (info.info != null) throw new RogueException();

            info.info = floor;
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public IDioramaFloor info;

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
