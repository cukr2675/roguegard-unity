using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class Within1TileRogueMethodRange : IRogueMethodRange
    {
        public static Within1TileRogueMethodRange Instance { get; } = new Within1TileRogueMethodRange();

        public string Name => "周囲1マス";
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        IRogueDetails IRogueDescription.Details => null;

        public void Predicate(
            IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, float visibleRadius, RectInt room)
        {
            var sqrVisibleRadius = visibleRadius * visibleRadius;
            for (int i = 0; i < 8; i++)
            {
                var direction = new RogueDirection(i);
                SpaceUtility.Raycast(self.Location, self.Position, direction, 1, false, true, false, out var hitObj, out var position, out _);
                if (hitObj == null) continue;

                var distance = position - self.Position;
                if (distance.sqrMagnitude < sqrVisibleRadius || room.Contains(position))
                {
                    predicator.Predicate(self, hitObj, self.Position);
                }
            }
        }

        public void Predicate(
            IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, Vector2Int targetPosition)
        {
            for (int i = 0; i < 8; i++)
            {
                var direction = new RogueDirection(i);
                SpaceUtility.Raycast(self.Location, self.Position, direction, 1, false, true, false, out var hitObj, out _, out _);
                if (hitObj == null) continue;

                predicator.Predicate(self, hitObj, targetPosition);
            }
        }
    }
}
