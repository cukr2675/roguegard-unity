using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class UserRogueMethodRange : IRogueMethodRange
    {
        public static UserRogueMethodRange Instance { get; } = new UserRogueMethodRange();

        public string Name => "自分";
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        object IRogueDescription.Details => null;

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
