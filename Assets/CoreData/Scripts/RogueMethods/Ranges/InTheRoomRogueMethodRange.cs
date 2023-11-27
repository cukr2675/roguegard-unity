using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class InTheRoomRogueMethodRange : IRogueMethodRange
    {
        public static InTheRoomRogueMethodRange Instance { get; } = new InTheRoomRogueMethodRange();

        public string Name => "部屋全体";
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        IRogueDetails IRogueDescription.Details => null;

        public void Predicate(
            IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, float visibleRadius, RectInt room)
        {
            var sqrVisibleRadius = visibleRadius * visibleRadius;
            var spaceObjs = self.Location.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var obj = spaceObjs[i];
                if (obj == null) continue;

                var distance = obj.Position - self.Position;
                if (distance.sqrMagnitude < sqrVisibleRadius || room.Contains(obj.Position))
                {
                    predicator.Predicate(self, obj, self.Position);
                }
            }
        }

        public void Predicate(
            IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, Vector2Int targetPosition)
        {
            var spaceObjs = self.Location.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var obj = spaceObjs[i];
                if (obj == null || obj.Position != targetPosition) continue;

                predicator.Predicate(self, obj, self.Position);
            }
        }
    }
}
