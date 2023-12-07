using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// ダンジョンを抜けると忘れるスキルなどを実装するための <see cref="IRogueObjInfo"/>
    /// </summary>
    public static class DungeonFloorCloserStateInfo
    {
        public static void AddTo(RogueObj obj, IDungeonFloorCloser closer)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }
            info.Add(closer);
        }

        public static bool ReplaceWithNull(RogueObj obj, IDungeonFloorCloser closer)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.ReplaceWithNull(closer);
            }
            return false;
        }

        public static void CloseAndRemoveNull(RogueObj obj, bool exitDungeon)
        {
            obj.Main.TryOpenRogueEffects(obj);

            if (obj.TryGet<Info>(out var dungeonInfo))
            {
                dungeonInfo.CloseAndRemoveNull(obj, exitDungeon);
            }
        }

        [ObjectFormer.IgnoreRequireRelationalComponent]
        private class Info : IRogueObjInfo
        {
            private readonly List<IDungeonFloorCloser> closers = new List<IDungeonFloorCloser>();

            bool IRogueObjInfo.IsExclusedWhenSerialize => true;

            public void Add(IDungeonFloorCloser closer)
            {
                closers.Add(closer);
            }

            public bool ReplaceWithNull(IDungeonFloorCloser closer)
            {
                var index = closers.IndexOf(closer);
                if (index >= 0)
                {
                    closers[index] = null;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void CloseAndRemoveNull(RogueObj self, bool exitDungeon)
            {
                for (int i = 0; i < closers.Count; i++)
                {
                    closers[i]?.RemoveClose(self, exitDungeon);
                    if (closers[i] == null)
                    {
                        closers.RemoveAt(i);
                        i--;
                    }
                }
            }

            bool IRogueObjInfo.CanStack(IRogueObjInfo other) => true;
            IRogueObjInfo IRogueObjInfo.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => new Info();
            IRogueObjInfo IRogueObjInfo.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
