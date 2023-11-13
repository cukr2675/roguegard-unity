using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class ShrinkedDungeonRoom
    {
        public RogueDungeonRoom BaseRoom { get; }
        public RectInt Rect { get; }

        private readonly List<int> rightCorridorPositions;
        private readonly List<int> leftCorridorPositions;
        private readonly List<int> upCorridorPositions;
        private readonly List<int> downCorridorPositions;

        private readonly List<int> rightVariablePositions;
        private readonly List<int> leftVariablePositions;
        private readonly List<int> upVariablePositions;
        private readonly List<int> downVariablePositions;

        private static readonly List<int> positions = new List<int>();
        private const int roomWallWidth = 1;

        public ShrinkedDungeonRoom(RogueDungeonRoom room, RectInt rect)
        {
            BaseRoom = room;
            Rect = rect;
            rightCorridorPositions = new List<int>();
            leftCorridorPositions = new List<int>();
            upCorridorPositions = new List<int>();
            downCorridorPositions = new List<int>();
            rightVariablePositions = new List<int>();
            leftVariablePositions = new List<int>();
            upVariablePositions = new List<int>();
            downVariablePositions = new List<int>();
            for (int i = roomWallWidth; i < Rect.height - roomWallWidth; i++)
            {
                rightVariablePositions.Add(i);
                leftVariablePositions.Add(i);
            }
            for (int i = roomWallWidth; i < Rect.width - roomWallWidth; i++)
            {
                upVariablePositions.Add(i);
                downVariablePositions.Add(i);
            }
        }

        public void SetTile(RogueTilemap tilemap, Spanning<IRogueTile> floorTiles, Spanning<IRogueTile> wallTiles)
        {
            // 壁（上下）
            var y0 = Rect.yMin;
            var y1 = Rect.yMax - 1;
            for (int x = Rect.xMin; x < Rect.xMax; x++)
            {
                tilemap.Replace(wallTiles, x, y0);
                tilemap.Replace(wallTiles, x, y1);
            }

            // 壁（左右）
            var x0 = Rect.xMin;
            var x1 = Rect.xMax - 1;
            for (int y = Rect.yMin + 1; y < Rect.yMax - 1; y++)
            {
                tilemap.Replace(wallTiles, x0, y);
                tilemap.Replace(wallTiles, x1, y);
            }

            // 床
            for (int y = Rect.yMin + 1; y < Rect.yMax - 1; y++)
            {
                for (int x = Rect.xMin + 1; x < Rect.xMax - 1; x++)
                {
                    tilemap.Replace(floorTiles, x, y);
                }
            }
        }

        public Vector2Int GetRightCorridorPosition(int corridorWidth, IRogueRandom random)
        {
            var position = GetCorridorPosition(corridorWidth, random, rightCorridorPositions, rightVariablePositions);
            return new Vector2Int(Rect.xMax - 1, Rect.yMin + position);
        }

        public Vector2Int GetLeftCorridorPosition(int corridorWidth, IRogueRandom random)
        {
            var position = GetCorridorPosition(corridorWidth, random, leftCorridorPositions, leftVariablePositions);
            return new Vector2Int(Rect.xMin, Rect.yMin + position);
        }

        public Vector2Int GetUpCorridorPosition(int corridorWidth, IRogueRandom random)
        {
            var position = GetCorridorPosition(corridorWidth, random, upCorridorPositions, upVariablePositions);
            return new Vector2Int(Rect.xMin + position, Rect.yMax - 1);
        }

        public Vector2Int GetDownCorridorPosition(int corridorWidth, IRogueRandom random)
        {
            var position = GetCorridorPosition(corridorWidth, random, downCorridorPositions, downVariablePositions);
            return new Vector2Int(Rect.xMin + position, Rect.yMin);
        }

        private int GetCorridorPosition(int corridorWidth, IRogueRandom random, List<int> corridorPositions, List<int> variablePositions)
        {
            UpdatePositions(corridorWidth, variablePositions);
            if (positions.Count >= 1)
            {
                // 新しい入口が生成されたとき、その位置はほかの入口に使えないようにする
                var position = random.Choice(positions);
                corridorPositions.Add(position);
                for (int i = -1; i < corridorWidth + 1; i++) // 入口に隣接した入口が生成できないように周囲1マスも消す
                {
                    variablePositions.Remove(position - i);
                }
                return position;
            }
            else if (corridorPositions.Count >= 1)
            {
                // 新しい入口を生成できないとき、既存の入口を使用する
                return random.Choice(corridorPositions);
            }
            else
            {
                // 入口の設定に十分な幅がないとき、最大位置を返す
                Debug.LogWarning("通路の幅が部屋の幅を超えています。");
                var position = variablePositions[variablePositions.Count - 1];
                corridorPositions.Add(position);
                variablePositions.Clear();
                return position;
            }
        }

        private static void UpdatePositions(int corridorWidth, List<int> variablePositions)
        {
            positions.Clear();
            var continuousCount = 1;
            if (variablePositions.Count >= 1 && continuousCount >= corridorWidth)
            {
                // 連続空き数が通路を作るのに十分な幅であれば、リストに追加する
                positions.Add(variablePositions[0]);
            }

            for (int i = 1; i < variablePositions.Count; i++)
            {
                var position = variablePositions[i];
                var oldPosition = variablePositions[i - 1];
                if (position == oldPosition + 1)
                {
                    // 空きが連続しているとき連続空き数を加算する
                    continuousCount++;
                    if (continuousCount >= corridorWidth)
                    {
                        // 連続空き数が通路を作るのに十分な幅であれば、リストに追加する
                        positions.Add(position);
                    }
                }
                else
                {
                    // 空きが連続していなければ連続空き数をリセット
                    continuousCount = 1;
                }
            }
        }
    }
}
