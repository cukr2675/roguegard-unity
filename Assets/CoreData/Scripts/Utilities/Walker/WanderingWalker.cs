using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class WanderingWalker
    {
        private Vector2Int lastTargetPosition = new Vector2Int(-1, -1);

        private Cell[][] tile;

        private RectInt rect;

        private List<Vector2Int> path;

        private Vector2Int[] deltas = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
        };

        [Objforming.CreateInstance]
        private WanderingWalker() { }

        public WanderingWalker(Vector2Int size)
        {
            tile = new Cell[size.y][];
            for (int y = 0; y < size.y; y++)
            {
                tile[y] = new Cell[size.x];
                for (int x = 0; x < size.x; x++)
                {
                    tile[y][x] = new Cell();
                }
            }
            rect = new RectInt(Vector2Int.zero, size);
            path = new List<Vector2Int>();
        }

        private bool SetRoute(RogueObj player, bool canWalkDiagonally, bool needsUpdateTile)
        {
            var updated = UpdateTile(player.Location.Space);
            if (needsUpdateTile && !updated) return false;

            path.Clear();
            var tilemap = player.Location.Space.Tilemap;
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    tile[y][x].Reset();
                }
            }

            var startPosition = player.Position;
            var forward = player.Main.Stats.Direction.Forward;
            var backPosition = startPosition - forward;

            tile[startPosition.y][startPosition.x].Open(0, -Vector2Int.one);
            var size = tilemap.Height * tilemap.Width;
            for (int i = 0; i < size; i++)
            {
                var min = int.MaxValue;
                var minPosition = Vector2Int.zero;
                for (int y = 0; y < tilemap.Height; y++)
                {
                    for (int x = 0; x < tilemap.Width; x++)
                    {
                        var cell = tile[y][x];
                        if (!cell.IsOpened || cell.IsClosed) continue;

                        if (cell.H < min)
                        {
                            min = cell.H;
                            minPosition = new Vector2Int(x, y);
                        }
                    }
                }
                if (min == int.MaxValue)
                {
                    // 全セルが閉じられていたとき中断する。
                    break;
                }

                var targetIsOpened = OpenAround(minPosition, out var targetPosition);
                if (targetIsOpened)
                {
                    SetPath(targetPosition);
                    return true;
                }
            }

            // 目標地点にたどり着けない場合
            path.Add(startPosition);
            if (CheckGoal(startPosition) && !player.Location.Space.CollideAt(startPosition + forward))
            {
                var tile = player.Location.Space.Tilemap.GetTop(startPosition + forward);
                if (tile.Info.Category != CategoryKw.Pool)
                {
                    // 通路にいて前進できる場合は前方が部屋かもしれないので前進する。
                    path.Add(startPosition + forward);
                    return true;
                }
            }

            // 通路でないなら引き返す。
            path.Add(backPosition);
            return true;

            throw new RogueException();

            bool OpenAround(Vector2Int position, out Vector2Int targetPosition)
            {
                var positionCell = tile[position.y][position.x];
                positionCell.Close();
                foreach (var deltaPosition in deltas)
                {
                    var index = position + deltaPosition;
                    if (index.x < 0 || tilemap.Width <= index.x || index.y < 0 || tilemap.Height <= index.y) continue;

                    // 斜め移動対策
                    if (deltaPosition.x != 0 && deltaPosition.y != 0)
                    {
                        if (!canWalkDiagonally) continue;
                        if (!tile[index.y][position.x].CanAcrossOn) continue;
                        if (!tile[position.y][index.x].CanAcrossOn) continue;
                    }

                    var cell = tile[index.y][index.x];
                    if (!cell.CanWalkOn || cell.IsClosed || index == backPosition) continue;

                    var s = positionCell.S + 1;
                    cell.Open(s, position);

                    targetPosition = index;
                    if (CheckGoal(index)) return true;
                }
                targetPosition = default;
                return false;
            }

            void SetPath(Vector2Int targetPosition)
            {
                path.Add(targetPosition);
                var cell = tile[targetPosition.y][targetPosition.x];
                var size = tilemap.Height * tilemap.Width;
                for (int i = 0; i < size; i++)
                {
                    path.Insert(0, cell.OpenedFrom);
                    if (cell.OpenedFrom == startPosition) break;

                    cell = tile[cell.OpenedFrom.y][cell.OpenedFrom.x];
                }
            }

            bool CheckGoal(Vector2Int index)
            {
                // 通路を目標地点とする
                return (CheckCollide(tile, index + Vector2Int.left) && CheckCollide(tile, index + Vector2Int.right)) ||
                    (CheckCollide(tile, index + Vector2Int.down) && CheckCollide(tile, index + Vector2Int.up));
            }

            bool CheckCollide(Cell[][] tile, Vector2Int index) => !rect.Contains(index) || !tile[index.y][index.x].CanWalkOn;
        }

        public Vector2Int GetWalk(RogueObj player, bool fear)
        {
            var canWalkDiagonally = false;
            bool stop;

            if (player.Location == null) return player.Position;

            var currentPosition = player.Position;
            if (!fear)
            {
                // 部屋の敵に近づく
                if (player.Location.Space.TryGetRoomView(currentPosition, out _, out var roomObjs))
                {
                    for (int i = 0; i < roomObjs.Count; i++)
                    {
                        var roomObj = roomObjs[i];
                        if (roomObj == null || !StatsEffectedValues.AreVS(player, roomObj)) continue;

                        lastTargetPosition = roomObj.Position;
                        stop = false;
                        return roomObj.Position;
                    }
                }

                // 視界の敵に近づく
                // 敵がプレイヤーを壁越しに察知して近づいてしまわないように視界距離は固定
                {
                    var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
                    var sqrVisibleRadius = visibleRadius * visibleRadius;
                    var locationObjs = player.Location.Space.Objs;
                    for (int i = 0; i < locationObjs.Count; i++)
                    {
                        var locationObj = locationObjs[i];
                        if (locationObj == null || !StatsEffectedValues.AreVS(player, locationObj)) continue;

                        var relativePosition = locationObj.Position - currentPosition;
                        if (relativePosition.sqrMagnitude >= sqrVisibleRadius) continue;

                        lastTargetPosition = locationObj.Position;
                        stop = false;
                        return locationObj.Position;
                    }
                }
            }
            if (lastTargetPosition.x >= 0 &&
                MovementUtility.TryGetApproachDirection(player, lastTargetPosition, true, out var direction))
            {
                stop = false;
                return currentPosition + direction.Forward;
            }
            lastTargetPosition = new Vector2Int(-1, -1);

            if (path.Count >= 1)
            {
                //if (currentPosition == path[path.Count - 1])
                //{
                //    stop = true;
                //    return Vector2Int.zero;
                //}

                for (int i = 0; i < path.Count - 1; i++)
                {
                    var pathPoint = path[i];
                    if (pathPoint != currentPosition) continue;

                    if (player.Location.Space.CollideAt(path[i + 1])) break; // 移動に失敗したらパスを再生成
                    return path[i + 1];
                }
            }

            if (!SetRoute(player, canWalkDiagonally, false))
            {
                stop = true;
                return Vector2Int.zero;
            }
            if (path.Count <= 1)
            {
                stop = true;
                return Vector2Int.zero;
            }
            for (int i = 0; i < path.Count - 1; i++)
            {
                var pathPoint = path[i];
                if (pathPoint != currentPosition) continue;

                stop = i == path.Count - 1;
                return path[i + 1];
            }

            throw new RogueException();
        }

        private bool UpdateTile(RogueSpace spaceStatus)
        {
            var updated = false;
            for (int y = 0; y < spaceStatus.Tilemap.Height; y++)
            {
                var tileRow = tile[y];
                for (int x = 0; x < spaceStatus.Tilemap.Width; x++)
                {
                    var cell = tileRow[x];
                    var collide = spaceStatus.CollideAt(new Vector2Int(x, y), true, true);
                    var tile = spaceStatus.Tilemap.GetTop(new Vector2Int(x, y));
                    var canWalkOn = !collide && tile.Info.Category != CategoryKw.Pool;
                    var canAcrossOn = canWalkOn || (tile != null ? tile.Info.Category == CategoryKw.Pool : false);
                    if (canWalkOn != cell.CanWalkOn || canAcrossOn != cell.CanAcrossOn)
                    {
                        cell.CanWalkOn = canWalkOn;
                        cell.CanAcrossOn = canAcrossOn;
                        updated = true;
                    }
                }
            }
            return updated;
        }

        [Objforming.Formable]
        private class Cell
        {
            public bool CanWalkOn { get; set; }
            public bool CanAcrossOn { get; set; }

            public int S { get; private set; }

            public int H => S;

            public Vector2Int OpenedFrom { get; private set; }

            public bool IsOpened { get; private set; }

            public bool IsClosed { get; private set; }

            public void Reset()
            {
                S = 0;
                IsOpened = false;
                IsClosed = false;
            }

            public void Open(int s, Vector2Int from)
            {
                if (!IsOpened || s < H)
                {
                    S = s;
                    OpenedFrom = from;
                    IsOpened = true;
                }
            }

            public void Close()
            {
                IsClosed = true;
            }
        }
    }
}
