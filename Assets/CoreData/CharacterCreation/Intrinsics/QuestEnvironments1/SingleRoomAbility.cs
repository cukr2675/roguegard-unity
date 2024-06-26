using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class SingleRoomAbility : PartyAbilityIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueMethodPassiveAspect
        {
            float IRogueMethodPassiveAspect.Order => 0f;

            private static readonly RectInt[] rooms = new RectInt[1];

            public SortedIntrinsic(int lv) : base(lv) { }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveChain chain)
            {
                var generate = keyword == MainInfoKw.Locate && self.Location.Space.Tilemap == null;

                var result = chain.Invoke(keyword, method, self, user, activationDepth, arg);
                if (!result) return false;

                if (generate && self.Location.Space.Tilemap != null && DungeonInfo.TryGet(self.Location, out var info) &&
                    info.TryGetFloor(0, out var level))
                {
                    // 階層を壁で埋める
                    var tilemap = self.Location.Space.Tilemap;
                    for (int y = 0; y < tilemap.Height; y++)
                    {
                        for (int x = 0; x < tilemap.Width; x++)
                        {
                            tilemap.Replace(level.FillTiles, x, y);
                        }
                    }

                    // 階層の部屋を 10x10 の一部屋だけにする
                    // プレイヤーが壁に埋まらないようにする
                    var size = new Vector2Int(Mathf.Min(10, tilemap.Width), Mathf.Min(10, tilemap.Height));
                    var random = RogueRandom.Primary;
                    var room = new RectInt(self.Position, size);
                    var xMax = Mathf.Max(tilemap.Width - size.x, 0);
                    room.x = Mathf.Clamp(room.x - 1 + random.Next(-size.x + 3, 0), 0, xMax); // プレイヤーが壁に埋まらないようにする
                    var yMax = Mathf.Max(tilemap.Height - size.y, 0);
                    room.y = Mathf.Clamp(room.y - 1 + random.Next(-size.y + 3, 0), 0, yMax);
                    for (int y = room.yMin; y < room.yMax; y++)
                    {
                        tilemap.Replace(level.RoomWallTiles, room.xMin, y);
                        tilemap.Replace(level.RoomWallTiles, room.xMax - 1, y);
                    }
                    for (int x = room.xMin + 1; x < room.xMax - 1; x++)
                    {
                        tilemap.Replace(level.RoomWallTiles, x, room.yMin);
                        tilemap.Replace(level.RoomWallTiles, x, room.yMax - 1);
                    }
                    for (int y = room.yMin + 1; y < room.yMax - 1; y++)
                    {
                        for (int x = room.xMin + 1; x < room.xMax - 1; x++)
                        {
                            tilemap.Replace(level.RoomGroundTiles, x, y);
                        }
                    }

                    rooms[0] = room;
                    self.Location.Space.SetRooms(rooms);
                }
                return true;
            }
        }
    }
}
