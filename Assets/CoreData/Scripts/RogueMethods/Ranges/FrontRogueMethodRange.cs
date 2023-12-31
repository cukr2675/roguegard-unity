using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class FrontRogueMethodRange : IProjectileRogueMethodRange
    {
        public static FrontRogueMethodRange Instance { get; } = new FrontRogueMethodRange();

        public string Name => "正面";
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        IRogueDetails IRogueDescription.Details => null;

        public void Predicate(
            IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, float visibleRadius, RectInt room)
        {
            var sqrVisibleRadius = visibleRadius * visibleRadius;
            var tileCollide = MovementCalculator.Get(tool ?? self).HasTileCollider;
            for (int i = 0; i < 8; i++)
            {
                var direction = new RogueDirection(i);
                SpaceUtility.Raycast(self.Location, self.Position, direction, 1, false, true, tileCollide, out var hitObj, out var position, out _);
                if (hitObj == null) continue;

                var distance = position - self.Position;
                if (distance.sqrMagnitude < sqrVisibleRadius || room.Contains(position))
                {
                    predicator.Predicate(self, hitObj, self.Position + direction.Forward);
                }
            }
        }

        public void Predicate(
            IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, Vector2Int targetPosition)
        {
            var tileCollide = MovementCalculator.Get(tool ?? self).HasTileCollider;
            if (!RogueDirection.TryFromSign(targetPosition - self.Position, out var direction)) return;

            SpaceUtility.Raycast(self.Location, self.Position, direction, 1, false, true, tileCollide, out var hitObj, out _, out _);
            if (hitObj == null) return;

            predicator.Predicate(self, hitObj, targetPosition);
        }

        bool IProjectileRogueMethodRange.Raycast(
            RogueObj space, Vector2Int origin, RogueDirection direction, bool collide, bool tileCollide,
            out RogueObj hitObj, out Vector2Int hitPosition, out Vector2Int dropPosition)
        {
            return SpaceUtility.Raycast(space, origin, direction, 1, false, collide, tileCollide, out hitObj, out hitPosition, out dropPosition);
        }
    }
}
