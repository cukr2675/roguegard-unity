using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    /// <summary>
    /// ダンジョンの階層データを公開するクラス
    /// </summary>
    public static class DungeonInfo
    {
        public static RogueObj GetLargestDungeon(RogueObj obj)
        {
            var location = obj;
            RogueObj dungeon = null;
            while (location != null)
            {
                if (TryGetLevel(location, 0, out _))
                {
                    dungeon = location;
                }
                location = location.Location;
            }
            return dungeon;
        }

        public static void SetLevelsTo(RogueObj dungeon, Spanning<RogueDungeonLevel> levels, DungeonLevelType levelType, float visibleRadius)
        {
            if (!dungeon.TryGet<Info>(out var info))
            {
                info = new Info();
                dungeon.SetInfo(info);
            }

            // 上書き不可
            if (info.levels != null) throw new RogueException();

            info.levels = levels.ToArray();
            info.levelType = levelType;
            info.visibleRadius = visibleRadius;
        }

        public static string GetLevelText(RogueObj dungeon)
        {
            var lv = dungeon.Main.Stats.Lv;
            dungeon.Main.TryOpenRogueEffects(dungeon);

            if (dungeon.TryGet<Info>(out var info))
            {
                return GetLevelText(info.levelType, lv);
            }
            return null;
        }

        public static string GetLevelText(DungeonLevelType levelType, int lv)
        {
            switch (levelType)
            {
                case DungeonLevelType.Down:
                    return $"B{lv}F";
                case DungeonLevelType.Up:
                    return $"{lv}F";
                default:
                    return null;
            }
        }

        public static float GetLocationVisibleRadius(RogueObj obj)
        {
            if (obj.Location.TryGet<Info>(out var info))
            {
                return info.visibleRadius;
            }
            return RoguegardSettings.DefaultVisibleRadius;
        }

        public static bool TryGetLevel(RogueObj dungeon, int lv, out RogueDungeonLevel level)
        {
            dungeon.Main.TryOpenRogueEffects(dungeon);

            if (dungeon.TryGet<Info>(out var info))
            {
                foreach (var item in info.levels)
                {
                    if (item.EndLv < lv) continue;

                    level = item;
                    return true;
                }
            }
            level = null;
            return false;
        }

        public static void SetSeedTo(RogueObj dungeon, int dungeonSeed)
        {
            if (!dungeon.TryGet<Info>(out var info))
            {
                info = new Info();
                dungeon.SetInfo(info);
            }

            // 上書き不可
            if (info.hasDungeonSeed) throw new RogueException();

            info.hasDungeonSeed = true;
            info.dungeonSeed = dungeonSeed;
        }

        public static bool TryGetRandom(RogueObj dungeon, int lv, out IRogueRandom random)
        {
            dungeon.Main.TryOpenRogueEffects(dungeon);

            if (dungeon.TryGet<Info>(out var info))
            {
                if (!info.hasDungeonSeed) throw new RogueException("このダンジョンはシード値を持っていません。");

                var floorSeed = info.GetFloorSeed(lv);
                random = new RogueRandom(floorSeed);
                return true;
            }
            random = null;
            return false;
        }

        [ObjectFormer.Formable]
        private class Info : IRogueObjInfo
        {
            [System.NonSerialized]
            public RogueDungeonLevel[] levels;

            [System.NonSerialized]
            public DungeonLevelType levelType;

            [System.NonSerialized]
            public float visibleRadius;

            [System.NonSerialized]
            private readonly List<int> floorSeeds = new List<int>();

            [System.NonSerialized]
            private RogueRandom random;

            public bool hasDungeonSeed;
            public int dungeonSeed;

            bool IRogueObjInfo.IsExclusedWhenSerialize => false;

            public int GetFloorSeed(int lv)
            {
                // すでに取得済みの lv のシード値であれば、キャッシュから返す
                if (lv < floorSeeds.Count) return floorSeeds[lv];

                // 初めて取得する lv のシード値の場合、新しく生成してキャッシュする
                // 乱数は一つずつ生成する必要があるため、指定の lv までのすべてを生成する
                random ??= new RogueRandom(dungeonSeed);
                for (int i = floorSeeds.Count; i <= lv; i++)
                {
                    var floorSeed = random.Next(int.MinValue, int.MaxValue);
                    floorSeeds.Add(floorSeed);
                }
                return floorSeeds[lv];
            }

            bool IRogueObjInfo.CanStack(IRogueObjInfo other) => true;
            IRogueObjInfo IRogueObjInfo.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            IRogueObjInfo IRogueObjInfo.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
