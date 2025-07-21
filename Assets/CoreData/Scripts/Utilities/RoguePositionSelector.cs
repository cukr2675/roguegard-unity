using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RoguePositionSelector
    {
        /// <summary>
        /// 位置候補リスト
        /// </summary>
        private readonly List<Vector2Int> possiblePositions = new();

        public void Update(IRogueTilemapView view)
        {
            possiblePositions.Clear();
            var size = view.Size;
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var position = new Vector2Int(x, y);
                    view.GetTile(position, out _, out var groundTile, out var buildingTile, out var tileObj);
                    var topTile = buildingTile ?? groundTile;
                    if (topTile == null && tileObj == null) continue;
                    if (topTile != null && (topTile.Info.HasCollider || topTile.Info.Category == CategoryKw.Pool)) continue;
                    if (tileObj != null && (tileObj.HasCollider || tileObj.Main.InfoSet.Category == CategoryKw.Pool)) continue;

                    if (!Mapped(view, x - 1, y) || !Mapped(view, x + 1, y) || !Mapped(view, x, y - 1) || !Mapped(view, x, y + 1))
                    {
                        possiblePositions.Add(position);
                    }
                }
            }
        }

        /// <summary>
        /// 指定の位置がすでにマッピングされているか取得する
        /// </summary>
        private static bool Mapped(IRogueTilemapView view, int x, int y) => Mapped(view, new Vector2Int(x, y));

        private static bool Mapped(IRogueTilemapView view, Vector2Int position)
        {
            view.GetTile(position, out _, out var groundTile, out var buildingTile, out var tileObj);
            return groundTile != null || buildingTile != null || tileObj != null;
        }

        public bool TryGetTargetPosition(Vector2Int currentPosition, out Vector2Int targetPosition)
        {
            // 優先度の計算
            // 近くてスコアが高いほど優先度が高い
            var maxScore = 0f;
            Vector2Int maxPosition = default;
            foreach (var possiblePosition in possiblePositions)
            {
                var relationalPosition = possiblePosition - currentPosition;
                var score = 10f / (Mathf.Abs(relationalPosition.x) + Mathf.Abs(relationalPosition.y));
                if (score > maxScore)
                {
                    maxScore = score;
                    maxPosition = possiblePosition;
                }
            }

            if (maxScore > 0f)
            {
                targetPosition = maxPosition;
                return true;
            }
            else
            {
                targetPosition = default;
                return false;
            }
        }
    }
}
