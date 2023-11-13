using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class SavePointInfo
    {
        public static ISavePointInfo Get(RogueObj obj)
        {
            obj.Main.TryOpenRogueEffects(obj);

            if (obj.TryGet<Info>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        public static void SetTo(RogueObj obj, ISavePointInfo info)
        {
            if (obj.TryGet<Info>(out _))
            {
                Debug.LogError("すでに設定されています。");
                return;
            }

            var objInfo = new Info();
            objInfo.info = info;
            obj.SetInfo(objInfo);
        }

        public static void RemoveFrom(RogueObj obj)
        {
            obj.RemoveInfo(typeof(Info));
        }

        [ObjectFormer.IgnoreRequireRelationalComponent]
        private class Info : IRogueObjInfo
        {
            public ISavePointInfo info;

            public bool IsExclusedWhenSerialize => true;

            public bool CanStack(IRogueObjInfo other) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj) => null;
        }
    }
}
