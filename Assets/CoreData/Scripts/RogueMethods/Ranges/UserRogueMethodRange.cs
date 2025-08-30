using UnityEngine;

namespace Roguegard
{
    public class UserRogueMethodRange : IRogueMethodRange
    {
        public static UserRogueMethodRange Instance { get; } = new UserRogueMethodRange();

        public string Name => "自分";
        Sprite IRogueDescribable.Icon => null;
        Color IRogueDescribable.Color => Color.white;
        string IRogueDescribable.Caption => null;
        IRogueDetails IRogueDescribable.Details => null;
        Spanning<IKeyword> IRogueDescribable.Tags => Spanning<IKeyword>.Empty;

        public void Predicate(
            IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, float visibleRadius, RectInt room)
        {
            predicator.Predicate(self, self, self.Position);
        }

        public void Predicate(
            IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj tool, Vector2Int targetPosition)
        {
            predicator.Predicate(self, self, targetPosition);
        }
    }
}
