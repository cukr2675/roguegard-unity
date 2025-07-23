using UnityEngine;

namespace Roguegard
{
    public class InTheRoomRogueMethodRange : IRogueMethodRange
    {
        public static InTheRoomRogueMethodRange Instance { get; } = new InTheRoomRogueMethodRange();

        public string Name => "部屋全体";
        Sprite IRogueDescribable.Icon => null;
        Color IRogueDescribable.Color => Color.white;
        string IRogueDescribable.Caption => null;
        IRogueDetails IRogueDescribable.Details => null;

        public void Predicate(
            IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, float visibleRadius, RectInt room)
        {
            var sqrVisibleRadius = visibleRadius * visibleRadius;
            foreach (var obj in self.Location.Space.Objs)
            {
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
            foreach (var obj in self.Location.Space.Objs)
            {
                if (obj == null || obj.Position != targetPosition) continue;

                predicator.Predicate(self, obj, self.Position);
            }
        }
    }
}
