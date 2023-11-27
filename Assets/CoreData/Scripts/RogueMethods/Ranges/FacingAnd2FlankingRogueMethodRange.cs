using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class FacingAnd2FlankingRogueMethodRange : IRogueMethodRange
    {
        public static FacingAnd2FlankingRogueMethodRange Instance { get; } = new FacingAnd2FlankingRogueMethodRange();

        public string Name => "正面3方向";
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        IRogueDetails IRogueDescription.Details => null;

        public void Predicate(IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, float visibleRadius, RectInt room)
        {
            var sqrVisibleRadius = visibleRadius * visibleRadius;
            var tileCollide = MovementCalculator.Get(tool ?? self).HasTileCollider;
            for (int i = 0; i < 8; i++)
            {
                var forward = new RogueDirection(i).Forward;

                // 正面 (forward) に対して3方向の判定
                // パワースラッシュのエフェクトと同じように左から右へ
                for (int j = +1; j >= -1; j--)
                {
                    var direction = new RogueDirection(i + j);
                    SpaceUtility.Raycast(self.Location, self.Position, direction, 1, false, true, tileCollide, out var hitObj, out var position, out _);
                    if (hitObj == null) continue;

                    var distance = position - self.Position;
                    if (distance.sqrMagnitude < sqrVisibleRadius || room.Contains(position))
                    {
                        predicator.Predicate(self, hitObj, self.Position + forward);
                    }
                }
            }
        }

        public void Predicate(IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, Vector2Int targetPosition)
        {
            var tileCollide = MovementCalculator.Get(tool ?? self).HasTileCollider;
            if (!RogueDirection.TryFromSign(targetPosition - self.Position, out var direction)) return;

            // 正面 (forward) に対して3方向の判定
            for (int j = -1; j <= 1; j++)
            {
                var currentDirection = direction.Rotate(j);
                SpaceUtility.Raycast(self.Location, self.Position, currentDirection, 1, false, true, tileCollide, out var hitObj, out _, out _);
                if (hitObj == null) continue;

                predicator.Predicate(self, hitObj, targetPosition);
            }
        }
    }
}
