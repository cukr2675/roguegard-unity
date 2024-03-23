using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/Dungeon/Levels/Goal")]
    public class GoalDungeonLevel : RogueDungeonLevel
    {
        [SerializeField] private RogueDungeonGenerator _dungeonGenerator = null;
        [Space]
        [SerializeField] private RandomRoomObjTable[] _items = null;
        [SerializeField] private RandomRoomObjTable _goalItem = null;

        public override Spanning<IRogueTile> FillTiles => _dungeonGenerator.FillTiles;
        public override Spanning<IRogueTile> NoizeTiles => _dungeonGenerator.NoiseTiles;
        public override Spanning<IRogueTile> RoomFloorTiles => _dungeonGenerator.RoomFloorTiles;
        public override Spanning<IRogueTile> RoomWallTiles => _dungeonGenerator.RoomWallTiles;

        public override Spanning<IWeightedRogueObjGeneratorList> EnemyTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;
        public override Spanning<IWeightedRogueObjGeneratorList> ItemTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;
        public override Spanning<IWeightedRogueObjGeneratorList> OtherTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        private static readonly Vector2Int[] itemPositions = new[]
        {
            new Vector2Int(-1, +1),
            new Vector2Int(+1, +1),
            new Vector2Int(-1, -1),
            new Vector2Int(+1, -1),
            new Vector2Int(+0, +0),
            new Vector2Int(+0, +1),
            new Vector2Int(-1, +0),
            new Vector2Int(+1, +0),
            new Vector2Int(+0, -1),
        };

        public override void GenerateFloor(RogueObj player, RogueObj floor, IRogueRandom random)
        {
            var dungeon = player.Location;
            var lv = dungeon.Main.Stats.Lv;

            // ダンジョン地形生成
            {
                floor.Main.Stats.SetLv(floor, lv);
                var rect = new RectInt(0, 0, 23, 16);
                var tilemap = new RogueTilemap(rect.size);
                var roomRect = new RectInt(8, 2, 7, 12);
                var wallRect = new RectInt(7, 1, 9, 14);
                for (int y = 0; y < rect.height; y++)
                {
                    for (int x = 0; x < rect.width; x++)
                    {
                        var position = new Vector2Int(x, y);
                        if (roomRect.Contains(position))
                        {
                            tilemap.Replace(_dungeonGenerator.RoomFloorTiles, x, y);
                        }
                        else if (wallRect.Contains(position))
                        {
                            tilemap.Replace(_dungeonGenerator.RoomWallTiles, x, y);
                        }
                        else
                        {
                            tilemap.Replace(_dungeonGenerator.FillTiles, x, y);
                        }
                    }
                }
                floor.Space.SetTilemap(tilemap);
                floor.Space.SetRooms(new[] { rect });
            }

            // プレイヤーキャラを移動
            {
                var position = floor.Space.Tilemap.Rect.size / 2 + Vector2Int.down * 5;
                //if (!SpaceUtility.TryLocate(player, floor, position)) { Debug.LogError("生成に失敗しました。"); }
                if (!default(IActiveRogueMethodCaller).Locate(player, null, floor, position, 0f)) { Debug.LogError("生成に失敗しました。"); }
            }
            // パーティメンバーを移動
            var party = player.Main.Stats.Party;
            var members = party.Members;
            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (default(IActiveRogueMethodCaller).LocateNextToAnyMember(member, null, 0f, party)) continue;

                // メンバーの移動に失敗したらランダム位置へ移動
                var position = floor.Space.GetRandomPositionInRoom(random);
                if (default(IActiveRogueMethodCaller).Locate(player, null, floor, position, 0f)) continue;

                Debug.LogError("生成に失敗しました。");
            }

            // アイテムを生成
            for (int i = 0; i < _items.Length; i++)
            {
                var position = floor.Space.Tilemap.Rect.size / 2 + GetItemPosition(i, _items.Length);
                _items[i].GenerateFloor(player, floor, position, random);
            }

            // ゴールを生成
            {
                var position = floor.Space.Tilemap.Rect.size / 2 + Vector2Int.up * 5;
                _goalItem.GenerateFloor(player, floor, position, random);
            }

            // 空間移動後は obj.Main.IsTicked = true になる。
            // フロアの IsTicked を true にすることで、フロア内の全オブジェクトが同時に行動開始できるようにする。
            // （プレイヤーパーティを IsTicked = false にする手もあるが、オブジェクトも設定しないといけないので面倒）
            floor.Main.IsTicked = true;
        }

        private static Vector2Int GetItemPosition(int index, int itemsCount)
        {
            if (itemsCount <= 1)
            {
                return Vector2Int.zero;
            }
            else if (itemsCount < itemPositions.Length)
            {
                return itemPositions[index];
            }
            throw new System.ArgumentOutOfRangeException(nameof(itemsCount));
        }
    }
}
