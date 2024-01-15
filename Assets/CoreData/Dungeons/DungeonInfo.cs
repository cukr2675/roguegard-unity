using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    /// <summary>
    /// ダンジョンの階層データを公開するクラス
    /// </summary>
    [ObjectFormer.Formable]
    public class DungeonInfo
    {
        [System.NonSerialized]
        public RogueDungeonLevel[] levels;

        [System.NonSerialized]
        public DungeonLevelType levelType;

        [System.NonSerialized]
        public float visibleRadius;

        private bool hasDungeonSeed;
        private int dungeonSeed;

        [System.NonSerialized]
        private readonly List<int> floorSeeds = new List<int>();

        [System.NonSerialized]
        private RogueRandom random;

        public string GetLevelText(RogueObj dungeon)
        {
            var lv = dungeon.Main.Stats.Lv;
            return GetLevelText(levelType, lv);
        }

        public bool TryGetLevel(int lv, out RogueDungeonLevel level)
        {
            foreach (var item in levels)
            {
                if (item.EndLv < lv) continue;

                level = item;
                return true;
            }
            level = null;
            return false;
        }

        public bool TryGetRandom(int lv, out IRogueRandom random)
        {
            if (!hasDungeonSeed) throw new RogueException("このダンジョンはシード値を持っていません。");

            var floorSeed = GetFloorSeed(lv);
            random = new RogueRandom(floorSeed);
            return true;
        }

        private int GetFloorSeed(int lv)
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

        public static DungeonInfo Get(RogueObj location)
        {
            location.Main.TryOpenRogueEffects(location);
            if (location.TryGet<Info>(out var value))
            {
                return value.info;
            }
            return null;
        }

        public static bool TryGet(RogueObj location, out DungeonInfo info)
        {
            location.Main.TryOpenRogueEffects(location);
            if (location.TryGet<Info>(out var value))
            {
                info = value.info;
                return true;
            }
            info = null;
            return false;
        }

        public static RogueObj GetLargestDungeon(RogueObj obj)
        {
            var location = obj;
            RogueObj dungeon = null;
            while (location != null)
            {
                if (TryGet(location, out _))
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
                info.info = new DungeonInfo();
                dungeon.SetInfo(info);
            }

            // 上書き不可
            if (info.info.levels != null) throw new RogueException();

            info.info.levels = levels.ToArray();
            info.info.levelType = levelType;
            info.info.visibleRadius = visibleRadius;
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
                return info.info.visibleRadius;
            }
            return RoguegardSettings.DefaultVisibleRadius;
        }

        public static void SetSeedTo(RogueObj dungeon, int dungeonSeed)
        {
            if (!dungeon.TryGet<Info>(out var info))
            {
                info = new Info();
                info.info = new DungeonInfo();
                dungeon.SetInfo(info);
            }

            // 上書き不可
            if (info.info.hasDungeonSeed) throw new RogueException();

            info.info.hasDungeonSeed = true;
            info.info.dungeonSeed = dungeonSeed;
        }

        [ObjectFormer.Formable]
        private class Info : IRogueObjInfo
        {
            public DungeonInfo info;

            bool IRogueObjInfo.IsExclusedWhenSerialize => false;

            bool IRogueObjInfo.CanStack(IRogueObjInfo other) => false;
            IRogueObjInfo IRogueObjInfo.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            IRogueObjInfo IRogueObjInfo.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
