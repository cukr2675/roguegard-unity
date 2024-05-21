using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Dungeon/Map/Generator")]
    public class RogueDungeonGenerator : ScriptableObject
    {
        [SerializeField] private Vector2Int _floorSize;
        [Space]
        [SerializeField] private int _minRooms;
        [SerializeField] private int _maxRooms;
        [Space]
        [SerializeField] private int _minRoomWidth;
        [SerializeField] private int _maxRoomWidth;
        [Space]
        [SerializeField] private int _minAdditionalCorridors;
        [SerializeField] private int _maxAdditionalCorridors;

        [Header("Tiles")]
        [SerializeField] private ScriptableRogueTile[] _fillTiles;
        public Spanning<IRogueTile> FillTiles => _fillTiles;
        [Space]
        [SerializeField] private ScriptableRogueTile[] _noiseTiles;
        public Spanning<IRogueTile> NoiseTiles => _noiseTiles;
        [SerializeField] private float _noiseTileThreshold;
        [SerializeField] private float _noiseTileScale;
        [Space]
        [SerializeField] private ScriptableRogueTile[] _roomGroundTiles;
        public Spanning<IRogueTile> RoomGroundTiles => _roomGroundTiles;
        [SerializeField] private ScriptableRogueTile[] _roomWallTiles;
        public Spanning<IRogueTile> RoomWallTiles => _roomWallTiles;
        [Space]
        [SerializeField] private TileArray[] _corridorTiles;
        private int CorridorWidth => _corridorTiles.Length;
        private Spanning<IRogueTile[]> CorridorTiles => _corridorTiles.Select(x => x.Array).ToArray();

        public void Generate(RogueSpace space, IRogueRandom random)
        {
            OnValidate();

            var builder = new RogueDungeonBuilder(_floorSize);

            // 部屋を分割・接続する
            var roomCount = random.Next(_minRooms, _maxRooms + 1);
            for (int i = 1; i < roomCount; i++)
            {
                if (!builder.TryDivideConnectedRoom(_minRoomWidth, CorridorWidth, random)) break;
            }

            // 部屋をつなぐ通路を追加で生成する
            var additionalCorridorCount = random.Next(_minAdditionalCorridors, _maxAdditionalCorridors);
            for (int i = 0; i < additionalCorridorCount; i++)
            {
                var roomIndex = random.Next(0, builder.Rooms.Count);
                builder.ConnectRooms(roomIndex, random);
            }

            // 各部屋のサイズを決める
            var shrinkedRooms = new ShrinkedDungeonRoom[builder.Rooms.Count];
            for (int i = 0; i < shrinkedRooms.Length; i++)
            {
                var room = builder.Rooms[i];
                var rect = room.GetRandomShrinkedRoom(_minRoomWidth, _maxRoomWidth, random);
                shrinkedRooms[i] = new ShrinkedDungeonRoom(room, rect);
            }

            // 各部屋の入口を決める
            var connections = new RogueDungeonConnection[builder.Connectors.Count];
            for (int i = 0; i < connections.Length; i++)
            {
                var connector = builder.Connectors[i];
                var room0Index = builder.Rooms.IndexOf(connector.Room0);
                var room0 = shrinkedRooms[room0Index];
                var room1Index = builder.Rooms.IndexOf(connector.Room1);
                var room1 = shrinkedRooms[room1Index];
                connections[i] = RogueDungeonConnection.Create(room0, room1, CorridorWidth, random);
            }



            // タイルを敷く
            var tilemap = new RogueTilemap(_floorSize);

            // 最初に全体に敷き詰める + パーリンノイズで池を作る
            var noiseX = random.NextFloat(0f, 256f);
            var noiseY = random.NextFloat(0f, 256f);
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    var perlinNoise = Mathf.PerlinNoise(noiseX + x * _noiseTileScale, noiseY + y * _noiseTileScale);
                    if (perlinNoise >= _noiseTileThreshold && _noiseTiles.Length >= 1)
                    {
                        tilemap.Replace(_noiseTiles, x, y);
                    }
                    else
                    {
                        tilemap.Replace(_fillTiles, x, y);
                    }
                }
            }
            foreach (var shrinkedRoom in shrinkedRooms)
            {
                shrinkedRoom.SetTile(tilemap, _roomGroundTiles, _roomWallTiles);
            }
            for (int i = 0; i < CorridorTiles.Count / 2; i++)
            {
                foreach (var connection in connections)
                {
                    connection.SetTile(tilemap, CorridorTiles, i);
                }
                foreach (var connection in connections)
                {
                    connection.SetTile(tilemap, CorridorTiles, CorridorTiles.Count - 1 - i);
                }
            }
            if (CorridorTiles.Count % 2 == 1)
            {
                foreach (var connection in connections)
                {
                    connection.SetTile(tilemap, CorridorTiles, CorridorTiles.Count / 2);
                }
            }

            var rects = shrinkedRooms.Select(x => x.Rect).ToArray();
            space.SetTilemap(tilemap);
            space.SetRooms(rects);
        }

        private void OnValidate()
        {
            // 壁を壊したときのため、床は必ず埋める
            if (!ContainsGround(_fillTiles)) { Debug.LogError($"[{this}] {nameof(_fillTiles)} に {RogueTileLayer.Ground} が含まれません。"); }
            if (!ContainsGround(_roomGroundTiles)) { Debug.LogError($"[{this}] {nameof(_roomGroundTiles)} に {RogueTileLayer.Ground} が含まれません。"); }
            if (!ContainsGround(_roomWallTiles)) { Debug.LogError($"[{this}] {nameof(_roomWallTiles)} に {RogueTileLayer.Ground} が含まれません。"); }
            for (int i = 0; i < _corridorTiles.Length; i++)
            {
                if (!ContainsGround(_corridorTiles[i])) { Debug.LogError($"[{this}] {nameof(_corridorTiles)}[{i}] に {RogueTileLayer.Ground} が含まれません。"); }
            }

            bool ContainsGround(Spanning<IRogueTile> tiles)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    if (tiles[i].Info.Layer == RogueTileLayer.Ground) return true;
                }
                return false;
            }
        }

        [System.Serializable]
        private class TileArray
        {
            [SerializeField] private RogueTileInfoData[] _tiles = null;

            public IRogueTile[] Array => _tiles;

            public static implicit operator Spanning<IRogueTile>(TileArray tiles) => tiles._tiles;
        }
    }
}
