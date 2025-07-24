using Roguegard.CharacterCreation;
using System.Collections.Generic;

namespace Roguegard
{
    /// <summary>
    /// ダンジョンの階層データを公開するクラス
    /// </summary>
    [Objforming.Formable]
    public class DungeonInfo
    {
        [System.NonSerialized]
        private DungeonFloorAsset[] floors;

        [System.NonSerialized]
        private DungeonLevelType levelType;

        [System.NonSerialized]
        private float visibleRadius;

        private bool hasDungeonSeed;
        private int dungeonSeed;

        [System.NonSerialized]
        private readonly List<int> floorSeeds = new();

        [System.NonSerialized]
        private RogueRandom random;

        public string GetLevelText(RogueObj dungeon)
        {
            var lv = dungeon.Main.Stats.Lv;
            return GetLevelText(levelType, lv);
        }

        public bool TryGetFloor(int lv, out DungeonFloorAsset floor)
        {
            foreach (var item in floors)
            {
                if (item.EndLv < lv) continue;

                floor = item;
                return true;
            }
            floor = null;
            return false;
        }

        public bool TryGetRandom(int lv, out IRogueRandom random)
        {
            if (!hasDungeonSeed) throw new System.InvalidOperationException("このダンジョンはシード値を持っていません。");

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

        public static void SetFloorsTo(RogueObj dungeon, Spanning<DungeonFloorAsset> floors, DungeonLevelType levelType, float visibleRadius)
        {
            if (!dungeon.TryGet<Info>(out var info))
            {
                info = new Info
                {
                    info = new DungeonInfo()
                };
                dungeon.SetInfo(info);
            }

            // 上書き不可
            if (info.info.floors != null) throw new System.InvalidOperationException();

            info.info.floors = floors.ToArray();
            info.info.levelType = levelType;
            info.info.visibleRadius = visibleRadius;
        }

        public static string GetLevelText(DungeonLevelType levelType, int lv)
        {
            return levelType switch
            {
                DungeonLevelType.Down => $"B{lv}F",
                DungeonLevelType.Up => $"{lv}F",
                _ => null,
            };
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
                dungeon.SetInfo(new Info
                {
                    info = new DungeonInfo()
                });
            }

            // 上書き不可
            if (info.info.hasDungeonSeed) throw new System.InvalidOperationException();

            info.info.hasDungeonSeed = true;
            info.info.dungeonSeed = dungeonSeed;
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public DungeonInfo info;

            bool IRogueObjInfo.IsExclusedWhenSerialize => false;

            bool IRogueObjInfo.CanStack(IRogueObjInfo coming) => false;
            IRogueObjInfo IRogueObjInfo.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            IRogueObjInfo IRogueObjInfo.ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
