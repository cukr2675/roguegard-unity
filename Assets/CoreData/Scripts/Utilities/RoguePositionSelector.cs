using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RoguePositionSelector
    {
        private readonly List<Item> items = new List<Item>();

        private const int length = 10;

        public void Update(IRogueTilemapView view)
        {
            items.Clear();
            var size = view.Size;
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    view.GetTile(new Vector2Int(x, y), out _, out var tile, out var tileObj);
                    if (tile == null && tileObj == null) continue;
                    if (tile != null && tile.Info.HasCollider) continue;
                    if (tileObj != null && tileObj.HasCollider) continue;

                    if (!Mapped(view, x - 1, y)) { items.Add(new Item(x - 1, y, RogueDirection.Left)); }
                    if (!Mapped(view, x + 1, y)) { items.Add(new Item(x + 1, y, RogueDirection.Right)); }
                    if (!Mapped(view, x, y - 1)) { items.Add(new Item(x, y - 1, RogueDirection.Down)); }
                    if (!Mapped(view, x, y + 1)) { items.Add(new Item(x, y + 1, RogueDirection.Up)); }
                }
            }
            UpdateCost(view);
        }

        private void UpdateCost(IRogueTilemapView view)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                item.Score = length;
                for (int j = 0; j < length; j++)
                {
                    var position = item.Position + item.Direction.Forward * j;
                    if (Mapped(view, position) || OnExtension(position, item.Direction))
                    {
                        item.Score = j;
                        break;
                    }
                }
                items[i] = item;
            }
        }

        /// <summary>
        /// 指定の位置がすでにマッピングされているか取得する
        /// </summary>
        private static bool Mapped(IRogueTilemapView view, int x, int y) => Mapped(view, new Vector2Int(x, y));

        private static bool Mapped(IRogueTilemapView view, Vector2Int position)
        {
            view.GetTile(position, out _, out var tile, out var tileObj);
            return tile != null || tileObj != null;
        }

        /// <summary>
        /// ほかの <see cref="Item"/> の延長線上にあるか取得する
        /// </summary>
        private bool OnExtension(Vector2Int position, RogueDirection direction)
        {
            if (direction == RogueDirection.Right || direction == RogueDirection.Left)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item.Position.x == position.x)
                    {
                        if (item.Position.y < position.y && item.Direction == RogueDirection.Up) return true;
                        if (item.Position.y > position.y && item.Direction == RogueDirection.Down) return true;
                    }
                }
            }
            if (direction == RogueDirection.Up || direction == RogueDirection.Down)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item.Position.y == position.y)
                    {
                        if (item.Position.x < position.x && item.Direction == RogueDirection.Right) return true;
                        if (item.Position.x > position.x && item.Direction == RogueDirection.Left) return true;
                    }
                }
            }
            return false;
        }

        public bool TryGetTargetPosition(Vector2Int currentPosition, out Vector2Int targetPosition)
        {
            // 優先度の計算
            // 近くてスコアが高いほど優先度が高い
            var maxScore = 0f;
            Item maxItem = default;
            foreach (var item in items)
            {
                var relationalPosition = item.Position - currentPosition;
                var score = (float)item.Score / Mathf.Max(Mathf.Abs(relationalPosition.x), Mathf.Abs(relationalPosition.y));
                if (score > maxScore)
                {
                    maxScore = score;
                    maxItem = item;
                }
            }

            if (maxScore >= 1)
            {
                targetPosition = maxItem.Position;
                return true;
            }
            else
            {
                targetPosition = default;
                return false;
            }
        }

        private struct Item
        {
            public Vector2Int Position { get; }
            public RogueDirection Direction { get; }
            public int Score { get; set; }

            public Item(int x, int y, RogueDirection direction)
            {
                Position = new Vector2Int(x, y);
                Direction = direction;
                Score = -1;
            }
        }
    }
}
