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
        public static RogueObjList GetStorage(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.storage;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 上書き不可
        /// </summary>
        public static void SetStorageTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            // 上書き不可
            if (info.storage != null) throw new RogueException();

            info.storage = new RogueObjList();
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            [System.NonSerialized]
            public IChestInfo info;

            public RogueObjList storage;

            public bool IsExclusedWhenSerialize => storage == null;

            public bool CanStack(IRogueObjInfo other)
            {
                return true;
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
