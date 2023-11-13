using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="ShrinkedDungeonRoom"/> 同士を接続するタイルを敷く位置情報を持つクラス
    /// </summary>
    public class RogueDungeonConnection
    {
        public Vector2Int Position0 { get; }
        public RectInt Corridor { get; }
        public Vector2Int Position1 { get; }

        public RogueDungeonConnection(Vector2Int position0, RectInt corridor, Vector2Int position1)
        {
            Position0 = position0;
            Corridor = corridor;
            Position1 = position1;
        }

        public static RogueDungeonConnection Create(ShrinkedDungeonRoom room0, ShrinkedDungeonRoom room1, int corridorWidth, IRogueRandom random)
        {
            if (!room0.BaseRoom.TryGetSharedCorridor(room1.BaseRoom, out var corridor))
                throw new System.ArgumentException("共通の通路を持つ部屋ではありません。");

            Vector2Int position0, position1;
            if (room0.BaseRoom.RightCorridor.Equals(corridor))
            {
                position0 = room0.GetRightCorridorPosition(corridorWidth, random);
                position1 = room1.GetLeftCorridorPosition(corridorWidth, random);
            }
            else if (room0.BaseRoom.LeftCorridor.Equals(corridor))
            {
                position0 = room0.GetLeftCorridorPosition(corridorWidth, random);
                position1 = room1.GetRightCorridorPosition(corridorWidth, random);
            }
            else if (room0.BaseRoom.UpCorridor.Equals(corridor))
            {
                position0 = room0.GetUpCorridorPosition(corridorWidth, random);
                position1 = room1.GetDownCorridorPosition(corridorWidth, random);
            }
            else if (room0.BaseRoom.DownCorridor.Equals(corridor))
            {
                position0 = room0.GetDownCorridorPosition(corridorWidth, random);
                position1 = room1.GetUpCorridorPosition(corridorWidth, random);
            }
            else
            {
                throw new RogueException();
            }
            return new RogueDungeonConnection(position0, corridor, position1);
        }

        public void SetTile(RogueTilemap tilemap, Spanning<IRogueTile[]> corridorTiles, int index)
        {
            if (Corridor.height == corridorTiles.Count)
            {
                // 通路が横長
                SetHorizontalCorridorTile(tilemap, corridorTiles, index);
                //SetEvenTile(tilemap, corridorTiles, index);
                SetPositionToHorizontalCorridorTile(tilemap, corridorTiles, index, Position0, Position1);
                SetPositionToHorizontalCorridorTile(tilemap, corridorTiles, index, Position1, Position0);
            }
            else if (Corridor.width == corridorTiles.Count)
            {
                // 通路が縦長
                SetVerticalCorridorTile(tilemap, corridorTiles, index);
                //SetEvenTile(tilemap, corridorTiles, index);
                SetPositionToVerticalCorridorTile(tilemap, corridorTiles, index, Position0, Position1);
                SetPositionToVerticalCorridorTile(tilemap, corridorTiles, index, Position1, Position0);
            }
            else
            {
                throw new RogueException();
            }
        }

        private void SetPositionToHorizontalCorridorTile(
            RogueTilemap tilemap, Spanning<IRogueTile[]> corridorTiles, int index, Vector2Int position, Vector2Int to)
        {
            var center = corridorTiles.Count / 2;
            var x = position.x + index - corridorTiles.Count + 1;
            var startY = Mathf.Min(position.y, Corridor.yMin + center);
            var endY = Mathf.Max(position.y, Corridor.yMin + center - 1);
            for (int y = startY; y <= endY; y++)
            {
                tilemap.Replace(corridorTiles[index], x, y);
            }
        }

        private void SetPositionToVerticalCorridorTile(
            RogueTilemap tilemap, Spanning<IRogueTile[]> corridorTiles, int index, Vector2Int position, Vector2Int to)
        {
            var center = corridorTiles.Count / 2;
            var y = position.y + index - corridorTiles.Count + 1;
            var startX = Mathf.Min(position.x, Corridor.xMin + center);
            var endX = Mathf.Max(position.x, Corridor.xMin + center - 1);
            for (int x = startX; x <= endX; x++)
            {
                tilemap.Replace(corridorTiles[index], x, y);
            }
        }

        private void SetHorizontalCorridorTile(RogueTilemap tilemap, Spanning<IRogueTile[]> corridorTiles, int index)
        {
            var center = corridorTiles.Count / 2;
            var even = (corridorTiles.Count + 1) % 2;
            //if (index == corridorTiles.Count / 2) { even = 0; }
            var side = index - center;
            if (corridorTiles.Count % 2 == 0 && side < 0) { side++; }
            var sideSign = System.Math.Sign(side);
            side = Mathf.Abs(side);

            var y = Corridor.yMin + index;
            var startX = Mathf.Min(Position0.x, Position1.x) - side - center;
            var endX = Mathf.Max(Position0.x, Position1.x) + side - center + even;
            for (int x = startX; x <= endX; x++) // まっすぐ敷く
            {
                tilemap.Replace(corridorTiles[index], x, y);
            }
            for (int i = 0; i < side; i++) // 両端を閉じる
            {
                tilemap.Replace(corridorTiles[index], startX, y - sideSign * (i + 1));
                tilemap.Replace(corridorTiles[index], endX, y - sideSign * (i + 1));
            }
        }

        private void SetVerticalCorridorTile(RogueTilemap tilemap, Spanning<IRogueTile[]> corridorTiles, int index)
        {
            var center = corridorTiles.Count / 2;
            var even = (corridorTiles.Count + 1) % 2;
            //if (index == corridorTiles.Count / 2) { even = 0; }
            var side = index - center;
            if (corridorTiles.Count % 2 == 0 && side < 0) { side++; }
            var sideSign = System.Math.Sign(side);
            side = Mathf.Abs(side);

            var x = Corridor.xMin + index;
            var startY = Mathf.Min(Position0.y, Position1.y) - side - center;
            var endY = Mathf.Max(Position0.y, Position1.y) + side - center + even;
            for (int y = startY; y <= endY; y++) // まっすぐ敷く
            {
                tilemap.Replace(corridorTiles[index], x, y);
            }
            for (int i = 0; i < side; i++) // 両端を閉じる
            {
                tilemap.Replace(corridorTiles[index], x - sideSign * (i + 1), startY);
                tilemap.Replace(corridorTiles[index], x - sideSign * (i + 1), endY);
            }
        }

        private void SetEvenTile(RogueTilemap tilemap, Spanning<IRogueTile[]> corridorTiles, int index)
        {
            if (corridorTiles.Count % 2 == 1 && index == corridorTiles.Count / 2 - 1) return;

            index = corridorTiles.Count / 2;
            if (Corridor.height == corridorTiles.Count)
            {
                // 通路が横長
                var y = Corridor.yMin + index;
                var x = Mathf.Min(Position0.x, Position1.x) - index;
                //var x = Mathf.Max(Position0.x, Position1.x) - 1;
                tilemap.Replace(corridorTiles[index - 1], x, y);
            }
            else if (Corridor.width == corridorTiles.Count)
            {
                // 通路が縦長
                var x = Corridor.xMin + index;
                var y = Mathf.Min(Position0.y, Position1.y) - index;
                //var y = Mathf.Max(Position0.y, Position1.y) - 1;
                tilemap.Replace(corridorTiles[index - 1], x, y);
            }
            else
            {
                throw new RogueException();
            }
        }
    }
}
