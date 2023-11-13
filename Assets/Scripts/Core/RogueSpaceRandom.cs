using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    internal class RogueSpaceRandom
    {
        private RectInt[] smallRooms;

        /// <summary>
        /// 部屋内の <see cref="RogueTileLayer.Floor"/> である面積。位置をランダムで取得するときに使う
        /// </summary>
        public int FloorInRoomCount { get; private set; }

        public RogueSpaceRandom(RogueSpace space)
        {
            Reset(space);
        }

        public Vector2Int GetRandomPositionInRoom(RogueSpace space, IRogueRandom random)
        {
            var positionIndex = random.Next(0, FloorInRoomCount);
            var index = 0;
            for (int y = 0; y < space.Tilemap.Height; y++)
            {
                for (int x = 0; x < space.Tilemap.Width; x++)
                {
                    var position = new Vector2Int(x, y);
                    var tile = space.Tilemap.GetTop(position);
                    if (!InAnyRoom(space, position) || space.CollideAt(position) || tile.Info.Layer != RogueTileLayer.Floor) continue;

                    if (index == positionIndex) return new Vector2Int(x, y);

                    index++;
                }
            }
            throw new RogueException(FloorInRoomCount.ToString());
        }

        public void Reset(RogueSpace space)
        {
            if (smallRooms == null || smallRooms.Length != space.RoomCount)
            {
                smallRooms = new RectInt[space.RoomCount];
                for (int i = 0; i < space.RoomCount; i++)
                {
                    space.GetRoom(i, out var room, out _);
                    smallRooms[i] = new RectInt(room.xMin + 1, room.yMin + 1, room.width - 2, room.height - 2);
                }
            }

            FloorInRoomCount = space.Tilemap.Width * space.Tilemap.Height;
            for (int y = 0; y < space.Tilemap.Height; y++)
            {
                for (int x = 0; x < space.Tilemap.Width; x++)
                {
                    var position = new Vector2Int(x, y);
                    var tile = space.Tilemap.GetTop(position);
                    if (!InAnyRoom(space, position) || space.CollideAt(position) || tile.Info.Layer != RogueTileLayer.Floor) FloorInRoomCount--;
                }
            }
        }

        private bool InAnyRoom(RogueSpace space, Vector2Int position)
        {
            for (int i = 0; i < smallRooms.Length; i++)
            {
                var room = smallRooms[i];
                if (!room.Contains(position)) continue;

                // 部屋の入口に隣接していたら false
                if (position.x == room.xMin && !space.CollideAt(position + Vector2Int.left, false)) return false;
                if (position.x == room.xMax - 1 && !space.CollideAt(position + Vector2Int.right, false)) return false;
                if (position.y == room.yMin && !space.CollideAt(position + Vector2Int.down, false)) return false;
                if (position.y == room.yMax - 1 && !space.CollideAt(position + Vector2Int.up, false)) return false;

                return true;
            }
            return false;
        }

        /// <summary>
        /// 当たり判定を持つオブジェクトかタイルを削除した直後に呼び出すメソッド。
        /// 削除によって当たり判定が空いた場合、ランダム移動先床数を可算する。
        /// </summary>
        public void AddRandomPosition(RogueSpace space, Vector2Int position)
        {
            // 当たり判定を持つオブジェクトかタイルを削除した後、他に当たり判定を持つものが存在した場合何もしない
            if (space.CollideAt(position)) return;

            // オブジェクトやタイルの移動処理をした後、床タイルが露出していない場合何もしない
            var tile = space.Tilemap.GetTop(position);
            if (tile.Info.Layer != RogueTileLayer.Floor) return;

            if (InAnyRoom(space, position)) FloorInRoomCount++;
        }

        /// <summary>
        /// 当たり判定を持つオブジェクトかタイルを設定する直前に呼び出すメソッド。
        /// 直後に判定すると、もともと当たり判定があったのか設定によって変更されたのかの区別ができないため、直前に判定する。
        /// </summary>
        public void RemoveRandomPosition(RogueSpace space, Vector2Int position)
        {
            // 当たり判定を持つオブジェクトかタイルを設定する前から、他に当たり判定を持つものが存在した場合何もしない
            if (space.CollideAt(position)) return;

            // オブジェクトやタイルの移動処理をする前から、床タイルが露出していない場合何もしない
            var tile = space.Tilemap.GetTop(position);
            if (tile.Info.Layer != RogueTileLayer.Floor) return;

            if (InAnyRoom(space, position)) FloorInRoomCount--;
        }
    }
}
