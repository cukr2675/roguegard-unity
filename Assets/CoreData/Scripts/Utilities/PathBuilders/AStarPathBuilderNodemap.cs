using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    internal class AStarPathBuilderNodemap
    {
        private readonly AStarPathBuilderNode[][] map;
        private readonly Vector2Int capacity;

        public RectInt Rect { get; private set; }

        public AStarPathBuilderNode this[Vector2Int position] => this[position.y, position.x];

        public AStarPathBuilderNode this[int y, int x]
        {
            get
            {
                if (!Rect.Contains(new Vector2Int(x, y))) throw new System.IndexOutOfRangeException();

                return map[y][x];
            }
        }

        public AStarPathBuilderNodemap(Vector2Int capacity)
        {
            this.capacity = capacity;
            map = new AStarPathBuilderNode[capacity.y][];
            for (int y = 0; y < capacity.y; y++)
            {
                map[y] = new AStarPathBuilderNode[capacity.x];
                for (int x = 0; x < capacity.x; x++)
                {
                    map[y][x] = new AStarPathBuilderNode();
                }
            }
        }

        public bool UpdateMap(RogueObj self)
        {
            IRogueTilemapView tilemap;
            if (self.TryGet<ViewInfo>(out var view)) { tilemap = view; }
            else { tilemap = self.Location.Space; }
            var newRect = new RectInt(Vector2Int.zero, tilemap.Size);
            if (newRect.width > capacity.x || newRect.height > capacity.y) throw new RogueException();

            var movement = MovementCalculator.Get(self);
            Rect = newRect;
            var updated = false;
            for (int y = 0; y < newRect.height; y++)
            {
                for (int x = 0; x < newRect.width; x++)
                {
                    var node = map[y][x];
                    tilemap.GetTile(new Vector2Int(x, y), out _, out var tile, out var tileObj);
                    var collide = GetCollider(tile, tileObj);
                    var cornerCollide = collide && (tile != null ? tile.Info.Category != CategoryKw.Pool : true);
                    if (collide != node.HasCollider || cornerCollide != node.HasCornerCollider)
                    {
                        node.HasCollider = collide;
                        node.HasCornerCollider = cornerCollide;
                        updated = true;
                    }
                }
            }
            return updated;

            bool GetCollider(IRogueTile tile, RogueObj tileObj)
            {
                if (tile == null && tileObj == null)
                {
                    // null は進入不可として扱う。
                    return false;
                }
                else if (tileObj != null)
                {
                    return tileObj.HasCollider;
                }
                else
                {
                    if (tile.Info.Category == CategoryKw.Pool && !movement.SubIs(StdKw.PoolMovement)) return true;

                    return tile.Info.HasCollider;
                }
            }
        }

        public void Reset()
        {
            for (int y = 0; y < Rect.height; y++)
            {
                for (int x = 0; x < Rect.width; x++)
                {
                    var node = map[y][x];
                    node.Reset();
                }
            }
        }

        public bool TryGetLowestFOpenNodePosition(out Vector2Int position)
        {
            var minF = int.MaxValue;
            position = Vector2Int.zero;
            for (int y = 0; y < Rect.height; y++)
            {
                for (int x = 0; x < Rect.width; x++)
                {
                    var node = map[y][x];
                    if (node.IsOpen && node.F < minF)
                    {
                        minF = node.F;
                        position = new Vector2Int(x, y);
                    }
                }
            }
            if (minF != int.MaxValue)
            {
                // targetPosition に最も近い位置を返す
                return true;
            }
            else
            {
                // 全ノードが閉じられていたとき false
                position = Vector2Int.zero;
                return false;
            }
        }
    }
}
