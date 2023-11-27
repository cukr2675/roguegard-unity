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
        public static void AddCloser(RogueObj obj, IDungeonFloorCloser closer)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }
            info.Add(closer);
        }

        public static void Close(RogueObj obj, bool exitDungeon)
        {
            obj.Main.TryOpenRogueEffects(obj);

            if (obj.TryGet<Info>(out var dungeonInfo))
            {
                dungeonInfo.Close(obj, exitDungeon);
                obj.RemoveInfo(typeof(Info));
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

            public void Close(RogueObj self, bool exitDungeon)
            {
                for (int i = 0; i < closers.Count; i++)
                {
                    if (closers[i].RemoveClose(self, exitDungeon))
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
