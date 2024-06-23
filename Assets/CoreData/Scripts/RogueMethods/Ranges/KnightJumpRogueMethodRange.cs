using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class KnightJumpRogueMethodRange : IRogueMethodRange
    {
        public static KnightJumpRogueMethodRange Instance { get; } = new KnightJumpRogueMethodRange();

        public string Name => "桂馬飛び";
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
                // 正面 (forward) に1マスと斜め右に1マス
                var direction = new RogueDirection(i);
                var hit = SpaceUtility.Raycast(self.Location, self.Position, direction, 1, true, false, tileCollide, out _, out _, out _);
                if (hit) continue; // 壁に当たったら終了

                SpaceUtility.Raycast(self.Location, self.Position + direction.Forward, direction.Rotate(-1), 1, true, true, tileCollide, out var hitObj, out var position, out _);
                if (hitObj == null) continue;

                var distance = position - self.Position;
                if (distance.sqrMagnitude < sqrVisibleRadius || room.Contains(position))
                {
                    predicator.Predicate(self, hitObj, self.Position + direction.Forward);
                }
            }
        }

        public void Predicate(IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, Vector2Int targetPosition)
        {
            var tileCollide = MovementCalculator.Get(tool ?? self).HasTileCollider;
            if (!RogueDirection.TryFromSign(targetPosition - self.Position, out var direction)) return;

            var hit = SpaceUtility.Raycast(self.Location, self.Position, direction, 1, true, false, tileCollide, out _, out _, out _);
            if (hit) return; // 壁に当たったら終了

            SpaceUtility.Raycast(self.Location, self.Position + direction.Forward, direction.Rotate(-1), 1, true, true, tileCollide, out var hitObj, out _, out _);
            if (hitObj == null) return;

            predicator.Predicate(self, hitObj, targetPosition);
        }
    }
}
