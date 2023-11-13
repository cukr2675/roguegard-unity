using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    internal class PointingWalker
    {
        private Cell[][] tile;

        private readonly RectInt rect;

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

        public WalkStopper WalkStopper { get; set; }

        public PointingWalker(Vector2Int size)
        {
            rect = new RectInt(Vector2Int.zero, size);
            tile = new Cell[size.y][];
            for (int y = 0; y < size.y; y++)
            {
                tile[y] = new Cell[size.x];
                for (int x = 0; x < size.x; x++)
                {
                    tile[y][x] = new Cell();
                }
            }
            path = new List<Vector2Int>();
        }

        public bool SetRoute(RogueObj player, Vector2Int targetPosition, bool canWalkDiagonally, bool needsUpdateTile)
        {
            var view = player.Get<ViewInfo>();
            var updated = UpdateTile(player, view);
            if (needsUpdateTile && !updated) return false;

            if (!rect.Contains(targetPosition) || !tile[targetPosition.y][targetPosition.x].CanWalkOn)
            {
                // 移動できない位置をポインティングした場合、パスを生成せずに直進する
                path.Clear();
                return false;
            }

            path.Clear();
            for (int y = 0; y < view.Height; y++)
            {
                var tileRow = tile[y];
                for (int x = 0; x < view.Width; x++)
                {
                    var cell = tileRow[x];
                    cell.Reset();
                }
            }

            var startPosition = player.Position;
            tile[startPosition.y][startPosition.x].Open(0, targetPosition - startPosition, -Vector2Int.one);
            var size = view.Height * view.Width;
            for (int i = 0; i < size; i++)
            {
                var min = int.MaxValue;
                var minPosition = Vector2Int.zero;
                for (int y = 0; y < view.Height; y++)
                {
                    var tileRow = tile[y];
                    for (int x = 0; x < view.Width; x++)
                    {
                        var cell = tileRow[x];
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

                var targetIsOpened = OpenAround(minPosition);
                if (targetIsOpened)
                {
                    SetPath(targetPosition);
                    return true;
                }
            }

            // 目標地点にたどり着けない場合、最も距離スコアの低い地点を目指す。
            {
                var min = int.MaxValue;
                var minS = int.MaxValue;
                var minPosition = Vector2Int.zero;
                for (int y = 0; y < view.Height; y++)
                {
                    var tileRow = tile[y];
                    for (int x = 0; x < view.Width; x++)
                    {
                        var cell = tileRow[x];
                        if (!cell.IsOpened) continue;

                        // 距離スコアが同じなら実スコアで判定する。
                        if (cell.C < min || (cell.C == min && cell.S < minS))
                        {
                            min = cell.C;
                            minS = cell.S;
                            minPosition = new Vector2Int(x, y);
                        }
                    }
                }
                if (min == int.MaxValue)
                {
                    // どのセルも初期値だったとき
                    throw new RogueException();
                }
                else
                {
                    if (startPosition == minPosition) return false;

                    SetPath(minPosition);
                    return true;
                }
            }
            throw new RogueException();

            bool OpenAround(Vector2Int position)
            {
                var positionCell = tile[position.y][position.x];
                positionCell.Close();
                foreach (var deltaPosition in deltas)
                {
                    var index = position + deltaPosition;
                    if (index.x < 0 || view.Width <= index.x || index.y < 0 || view.Height <= index.y) continue;

                    // 斜め移動対策
                    if (deltaPosition.x != 0 && deltaPosition.y != 0)
                    {
                        if (!canWalkDiagonally) continue;
                        if (!tile[index.y][position.x].CanAcrossOn) continue;
                        if (!tile[position.y][index.x].CanAcrossOn) continue;
                    }

                    var cell = tile[index.y][index.x];
                    if (!cell.CanWalkOn || cell.IsClosed) continue;

                    var relativePosition = targetPosition - index;
                    var s = positionCell.S + 1;
                    cell.Open(s, relativePosition, position);

                    if (index == targetPosition) return true;
                }
                return false;
            }

            void SetPath(Vector2Int targetPosition)
            {
                path.Add(targetPosition);
                var cell = tile[targetPosition.y][targetPosition.x];
                var size = view.Height * view.Width;
                for (int i = 0; i < size; i++)
                {
                    path.Insert(0, cell.OpenedFrom);
                    if (cell.OpenedFrom == startPosition) break;

                    cell = tile[cell.OpenedFrom.y][cell.OpenedFrom.x];
                }
            }
        }

        public bool GetWalk(RogueObj player, Vector2Int targetPosition, bool canWalkDiagonally, out Vector2Int point, bool force)
        {
            point = default;
            WalkStopper.UpdateStatedStop();
            var checkView = WalkStopper.StatedStop;
            var currentPosition = player.Position;

            if (checkView && !force) return false;
            if (currentPosition == targetPosition) return false;
            if (path.Count == 0 && MovementUtility.TryGetApproachDirection(player, targetPosition, true, out var direction))
            {
                // パスがない場合は、移動できない位置をポインティングされていると判断して直進する
                var pathPoint = currentPosition + direction.Forward;

                WalkStopper.UpdatePositionedStop(pathPoint);
                if (WalkStopper.PositionedStop && !force) return false;

                point = pathPoint;
                return true;
            }
            else
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    var pathPoint = path[i];
                    if (pathPoint != currentPosition) continue;

                    WalkStopper.UpdatePositionedStop(path[i + 1]);
                    if (WalkStopper.PositionedStop && !force) return false;

                    point = path[i + 1];
                    return true;
                }

                if (force && (targetPosition - currentPosition).sqrMagnitude <= 2)
                {
                    point = targetPosition;
                    return true;
                }
            }

            if (!SetRoute(player, targetPosition, canWalkDiagonally, true)) return false;
            if (path.Count <= 1) return false;
            for (int i = 0; i < path.Count - 1; i++)
            {
                var pathPoint = path[i];
                if (pathPoint != currentPosition) continue;

                WalkStopper.UpdatePositionedStop(path[i + 1]);
                if (WalkStopper.PositionedStop && !force) return false;

                point = path[i + 1];
                return true;
            }

            Debug.LogError($"{nameof(PointingWalker)}エラー");
            return false;
        }

        private bool UpdateTile(RogueObj player, ViewInfo view)
        {
            var movement = MovementCalculator.Get(player);
            var updated = false;
            for (int y = 0; y < view.Height; y++)
            {
                var tileRow = tile[y];
                for (int x = 0; x < view.Width; x++)
                {
                    var cell = tileRow[x];
                    view.GetTile(new Vector2Int(x, y), out var visible, out var tile, out var tileObj);
                    var canWalkOn = GetCanWalkOn(tile, tileObj);
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

            bool GetCanWalkOn(IRogueTile tile, RogueObj tileObj)
            {
                if (tile == null)
                {
                    if (tileObj == null)
                    {
                        // null は進入不可として扱う。
                        return false;
                    }
                    else
                    {
                        return !tileObj.HasCollider;
                    }
                }
                else
                {
                    if (tile.Info.Category == CategoryKw.Pool && !movement.SubIs(StdKw.PoolMovement)) return false;

                    return !tile.Info.HasCollider;
                }
            }
        }

        private class Cell
        {
            public bool CanWalkOn { get; set; }
            public bool CanAcrossOn { get; set; }

            public int S { get; private set; }

            public int C { get; private set; }

            public int H => S + C;

            public Vector2Int OpenedFrom { get; private set; }

            public bool IsOpened { get; private set; }

            public bool IsClosed { get; private set; }

            public void Reset()
            {
                S = 0;
                C = 0;
                IsOpened = false;
                IsClosed = false;
            }

            public void Open(int s, Vector2Int relativePosition, Vector2Int from)
            {
                relativePosition.x = Mathf.Abs(relativePosition.x);
                relativePosition.y = Mathf.Abs(relativePosition.y);
                var c = relativePosition.x + relativePosition.y;

                if (!IsOpened || s + c < H)
                {
                    S = s;
                    C = c;
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
