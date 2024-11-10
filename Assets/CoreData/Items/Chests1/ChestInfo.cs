using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class ChestInfo
    {
        public static IChestInfo GetInfo(RogueObj obj)
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

        /// <summary>
        /// 上書き不可
        /// </summary>
        public static void SetInfoTo(RogueObj obj, IChestInfo chestInfo)
        {
            if (chestInfo == null) throw new System.ArgumentNullException(nameof(chestInfo));

            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            // 上書き不可
            if (info.info != null) throw new RogueException();

            info.info = chestInfo;
        }

        public static void RemoveFrom(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                info.info = null;
            }
        }

        [Objforming.IgnoreRequireRelationalComponent]
        private class Info : IRogueObjInfo
        {
            [System.NonSerialized]
            public IChestInfo info;

            // RogueObj.Space ではないストレージ用リストは使用不可能
            // ロビーメンバーの呼び戻し機能などで中のオブジェクトがひとりでに移動することがあり、
            // その場合はストレージが空間移動を検知できない（同じオブジェクトがストレージ内外で重複できてしまう）ため、
            // 必ず RogueObj.Space で管理する必要がある
            //public RogueObjList storage;

            public bool IsExclusedWhenSerialize => true;

            public bool CanStack(IRogueObjInfo coming) => true;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
