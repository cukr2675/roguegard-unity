using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard
{
    public class RogueDungeonRoomsBuilder
    {
        private readonly List<RogueDungeonRoom> _rooms;
        public Spanning<RogueDungeonRoom> Rooms => _rooms;

        private readonly List<RectInt> corridors;

        private static readonly List<int> dividableRoomIndices = new List<int>();
        private static readonly List<Direction> directions = new List<Direction>();

        public RogueDungeonRoomsBuilder(Vector2Int size)
        {
            _rooms = new List<RogueDungeonRoom>();
            corridors = new List<RectInt>();

            var bigRoom = new RogueDungeonRoom(Vector2Int.zero, size);
            _rooms.Add(bigRoom);
        }

        public void HorizontalDivideRoom(int roomIndex, int corridorYMin, int corridorHeight)
        {
            if (roomIndex >= _rooms.Count) throw new System.ArgumentOutOfRangeException(nameof(roomIndex));

            var room = _rooms[roomIndex];
            room.HorizontalDivide(corridorYMin, corridorHeight, out var downRoom, out var corridor, out var upRoom);

            _rooms[roomIndex] = downRoom;
            _rooms.Insert(roomIndex + 1, upRoom);
            corridors.Add(corridor);
        }

        public void VerticalDivideRoom(int roomIndex, int corridorXMin, int corridorWidth)
        {
            if (roomIndex >= _rooms.Count) throw new System.ArgumentOutOfRangeException(nameof(roomIndex));

            var room = _rooms[roomIndex];
            room.VerticalDivide(corridorXMin, corridorWidth, out var leftRoom, out var corridor, out var rightRoom);

            _rooms[roomIndex] = leftRoom;
            _rooms.Insert(roomIndex + 1, rightRoom);
            corridors.Add(corridor);
        }

        /// <summary>
        /// すべての部屋の中からランダムで分割する
        /// </summary>
        public bool TryDivideRandomRoom(
            int minRoomWidth, int corridorWidth, IRogueRandom random,
            out RogueDungeonRoom baseRoom, out RogueDungeonRoom dividedRoom0, out RogueDungeonRoom dividedRoom1)
        {
            // 分割できる部屋のリストを作る
            var mustWidth = minRoomWidth + corridorWidth + minRoomWidth;
            dividableRoomIndices.Clear();
            for (int i = 0; i < _rooms.Count; i++)
            {
                var room = _rooms[i];
                if (room.Width >= mustWidth || room.Height >= mustWidth) { dividableRoomIndices.Add(i); }
            }
            if (dividableRoomIndices.Count <= 0)
            {
                baseRoom = dividedRoom0 = dividedRoom1 = null;
                return false;
            }

            // リストから分割する部屋をランダムで決定する
            var roomIndex = random.Choice(dividableRoomIndices);
            baseRoom = _rooms[roomIndex];

            // 縦横どちらに分割するかランダムで決定する
            var canHorizontalDivide = baseRoom.Height >= mustWidth;
            var canVerticalDivide = baseRoom.Width >= mustWidth;
            if (canHorizontalDivide && canVerticalDivide)
            {
                var axis = random.Next(0, 2);
                if (axis == 0) DivideRoomAtRandomY(roomIndex, minRoomWidth, corridorWidth, random);
                else DivideRoomAtRandomX(roomIndex, minRoomWidth, corridorWidth, random);
            }
            else if (canHorizontalDivide && !canVerticalDivide)
            {
                DivideRoomAtRandomY(roomIndex, minRoomWidth, corridorWidth, random);
            }
            else if (!canHorizontalDivide && canVerticalDivide)
            {
                DivideRoomAtRandomX(roomIndex, minRoomWidth, corridorWidth, random);
            }
            else
            {
                throw new RogueException("縦にも横にも分割できません。");
            }
            dividedRoom0 = _rooms[roomIndex];
            dividedRoom1 = _rooms[roomIndex + 1];
            return true;
        }

        private void DivideRoomAtRandomY(int roomIndex, int minRoomWidth, int corridorWidth, IRogueRandom random)
        {
            var room = _rooms[roomIndex];
            var mustWidth = minRoomWidth + corridorWidth + minRoomWidth;
            var randomCorridorYMinWidth = room.Height - mustWidth;
            var corridorYMin = minRoomWidth + random.Next(0, randomCorridorYMinWidth + 1);
            HorizontalDivideRoom(roomIndex, corridorYMin, corridorWidth);
        }

        private void DivideRoomAtRandomX(int roomIndex, int minRoomWidth, int corridorWidth, IRogueRandom random)
        {
            var room = _rooms[roomIndex];
            var mustWidth = minRoomWidth + corridorWidth + minRoomWidth;
            var randomCorridorYMinWidth = room.Width - mustWidth;
            var corridorXMin = minRoomWidth + random.Next(0, randomCorridorYMinWidth + 1);
            VerticalDivideRoom(roomIndex, corridorXMin, corridorWidth);
        }

        /// <summary>
        /// <paramref name="roomIndex"/> の部屋から通路で接続できる部屋をランダムで取得する
        /// </summary>
        public bool TryGetRandomRoomFrom(int roomIndex, IRogueRandom random, out RogueDungeonRoom connectRoom)
        {
            var room = _rooms[roomIndex];

            directions.Clear();
            if (room.RightCorridor.size != default) { directions.Add(Direction.Right); }
            if (room.LeftCorridor.size != default) { directions.Add(Direction.Left); }
            if (room.UpCorridor.size != default) { directions.Add(Direction.Up); }
            if (room.DownCorridor.size != default) { directions.Add(Direction.Down); }
            if (directions.Count == 0)
            {
                connectRoom = null;
                return false;
            }

            // 通路に接続できる方向のうち、ランダムな方向を決定する
            var direction = random.Choice(directions);

            // 決定した方向から接続する部屋をランダムで取得する
            RogueDungeonRoom[] connectRooms;
            switch (direction)
            {
                case Direction.Right:
                    connectRooms = GetConnectedRoomsFromLeft(room.RightCorridor);
                    break;
                case Direction.Left:
                    connectRooms = GetConnectedRoomsFromRight(room.LeftCorridor);
                    break;
                case Direction.Up:
                    connectRooms = GetConnectedRoomsFromDown(room.UpCorridor);
                    break;
                case Direction.Down:
                    connectRooms = GetConnectedRoomsFromUp(room.DownCorridor);
                    break;
                default:
                    throw new RogueException();
            }
            connectRoom = random.Choice(connectRooms);
            return true;
        }

        private RogueDungeonRoom[] GetConnectedRoomsFromRight(RectInt corridor) => _rooms.Where(x => x.RightCorridor.Equals(corridor)).ToArray();
        private RogueDungeonRoom[] GetConnectedRoomsFromLeft(RectInt corridor) => _rooms.Where(x => x.LeftCorridor.Equals(corridor)).ToArray();
        private RogueDungeonRoom[] GetConnectedRoomsFromUp(RectInt corridor) => _rooms.Where(x => x.UpCorridor.Equals(corridor)).ToArray();
        private RogueDungeonRoom[] GetConnectedRoomsFromDown(RectInt corridor) => _rooms.Where(x => x.DownCorridor.Equals(corridor)).ToArray();

        private enum Direction
        {
            Right,
            Left,
            Up,
            Down
        }
    }
}
