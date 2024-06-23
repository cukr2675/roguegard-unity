using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class StraightLine10RogueMethodRange : IRogueMethodRange
    {
        public static StraightLine10RogueMethodRange Instance { get; } = new StraightLine10RogueMethodRange();

        public string Name => "直線10マス貫通";
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => "直線上10マスを貫く 障害物があるとそこで止まる";
        public IRogueDetails Details => null;

        public void Predicate(
            IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, float visibleRadius, RectInt room)
        {
            var sqrVisibleRadius = visibleRadius * visibleRadius;
            var tileCollide = MovementCalculator.Get(tool ?? self).HasTileCollider;
            var locationSpace = self.Location.Space;
            for (int i = 0; i < 8; i++)
            {
                var direction = new RogueDirection(i);
                var origin = self.Position + direction.Forward;
                for (int j = 0; j < 10; j++)
                {
                    var nextPosition = origin + direction.Forward * j;
                    if (!locationSpace.CollideAt(nextPosition, true, tileCollide)) continue;

                    var hitObj = locationSpace.GetColliderObj(nextPosition);
                    if (hitObj == null) continue;

                    var distance = nextPosition - self.Position;
                    if (distance.sqrMagnitude < sqrVisibleRadius || room.Contains(nextPosition))
                    {
                        predicator.Predicate(self, hitObj, self.Position + direction.Forward);
                    }
                }
            }
        }

        public void Predicate(
            IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, Vector2Int targetPosition)
        {
            var tileCollide = MovementCalculator.Get(tool ?? self).HasTileCollider;
            if (!RogueDirection.TryFromSign(targetPosition - self.Position, out var direction)) return;

            var locationSpace = self.Location.Space;
            var origin = self.Position + direction.Forward;
            for (int j = 0; j < 10; j++)
            {
                var nextPosition = origin + direction.Forward * j;
                if (!locationSpace.CollideAt(nextPosition, true, tileCollide)) continue;

                var hitObj = locationSpace.GetColliderObj(nextPosition);
                if (hitObj == null) continue;

                predicator.Predicate(self, hitObj, targetPosition);
            }
        }
    }
}
