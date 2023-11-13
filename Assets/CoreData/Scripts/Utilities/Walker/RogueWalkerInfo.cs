using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RogueWalkerInfo
    {
        public static IRogueWalker Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.Walker;
            }
            else
            {
                return null;
            }
        }

        public static void SetTo(RogueObj obj, IRogueWalker walker)
        {
            if (obj.TryGet<Info>(out _))
            {
                Debug.LogError("すでに設定されています。");
                return;
            }

            var info = new Info();
            info.Walker = walker;
            obj.SetInfo(info);
        }

        public static void RemoveFrom(RogueObj obj)
        {
            obj.RemoveInfo(typeof(Info));
        }

        [ObjectFormer.Formable]
        private class Info : IRogueObjInfo
        {
            public IRogueWalker Walker { get; set; }

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj) => null;
        }
    }
}
