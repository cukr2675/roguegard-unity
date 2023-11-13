using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RogueDungeonBuilder
    {
        private readonly RogueDungeonRoomsBuilder roomsBuilder;
        public Spanning<RogueDungeonRoom> Rooms => roomsBuilder.Rooms;

        private readonly List<RogueDungeonConnector> _connectors;
        public Spanning<RogueDungeonConnector> Connectors => _connectors;

        public RogueDungeonBuilder(Vector2Int size)
        {
            roomsBuilder = new RogueDungeonRoomsBuilder(size);
            _connectors = new List<RogueDungeonConnector>();
        }

        public bool TryDivideConnectedRoom(int minRoomWidth, int corridorWidth, IRogueRandom random)
        {
            // 部屋を分割する
            if (!roomsBuilder.TryDivideRandomRoom(
                minRoomWidth, corridorWidth, random,
                out RogueDungeonRoom baseRoom, out RogueDungeonRoom room0, out RogueDungeonRoom room1)) return false;

            // 分割された部屋への接続を分割後の部屋へ再設定する
            foreach (var connector in _connectors)
            {
                if (connector.Room0 == baseRoom) { connector.SetRoom0(room0, room1, random); }
                if (connector.Room1 == baseRoom) { connector.SetRoom1(room0, room1, random); }
            }
            var any0 = ConnectAny(room0);
            var any1 = ConnectAny(room1);
            if (any0 && any1)
            {
                // 分割後の部屋がどちらもどこかに接続されている場合、分割した部屋同士をつなぐ
                // （必ずしも分割した部屋同士である必要はなく、部屋群の分離さえ発生しなければどこでもよいが、実装が簡潔なためこうする）
                _connectors.Add(new RogueDungeonConnector(room0, room1));
            }
            else if (any0 && !any1)
            {
                // 片方だけ接続されている場合、接続されていないほうの部屋をどこかにつなぐ
                var room1Index = roomsBuilder.Rooms.IndexOf(room1);
                ConnectRooms(room1Index, random);
            }
            else if (!any0 && any1)
            {
                // 同上
                var room0Index = roomsBuilder.Rooms.IndexOf(room0);
                ConnectRooms(room0Index, random);
            }
            else if (roomsBuilder.Rooms.Count == 2)
            {
                // 最初の分割では分割後の部屋同士をつなぐ（最初の分割ではどちらの部屋も接続を持たない）
                _connectors.Add(new RogueDungeonConnector(room0, room1));
            }
            else
            {
                Debug.LogError("不正なダンジョン生成処理です。");
            }
            return true;
        }

        public void ConnectRooms(int roomIndex, IRogueRandom random)
        {
            var room = roomsBuilder.Rooms[roomIndex];
            if (roomsBuilder.TryGetRandomRoomFrom(roomIndex, random, out var connectRoom))
            {
                _connectors.Add(new RogueDungeonConnector(room, connectRoom));
            }
            else if (roomsBuilder.Rooms.Count != 1)
            {
                Debug.LogError("不正なダンジョン生成処理です。通路のない部屋が発生している可能性があります。");
            }
        }

        private bool ConnectAny(RogueDungeonRoom room)
        {
            foreach (var connector in _connectors)
            {
                if (connector.Room0 == room || connector.Room1 == room) return true;
            }
            return false;
        }
    }
}
