using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class ForPartyMemberRogueMethodTarget : IRogueMethodTarget
    {
        public static ForPartyMemberRogueMethodTarget Instance { get; } = new ForPartyMemberRogueMethodTarget();

        public string Name => "味方";
        Sprite IRogueDescribable.Icon => null;
        Color IRogueDescribable.Color => Color.white;
        string IRogueDescribable.Caption => null;
        IRogueDetails IRogueDescribable.Details => null;

        private static readonly Stack<Predicator> predicators = new();

        public IRoguePredicator GetPredicator(RogueObj self, float predictionDepth, RogueObj tool)
        {
            if (!predicators.TryPop(out var predicator))
            {
                predicator = new Predicator();
            }

            predicator.enemyPositions.Clear();
            predicator.partyMemberPositions.Clear();
            return predicator;
        }

        private class Predicator : IRoguePredicator
        {
            public readonly List<Vector2Int> enemyPositions = new();
            public readonly RoguePredicatorPositionList partyMemberPositions = new();

            public Spanning<Vector2Int> Positions => partyMemberPositions.Positions;

            public Spanning<RogueObj> GetObjs(Vector2Int position)
            {
                return partyMemberPositions.GetObjs(position);
            }

            public void Predicate(RogueObj self, RogueObj target, Vector2Int position)
            {
                if (StatsEffectedValues.AreVS(self, target))
                {
                    if (!enemyPositions.Contains(position)) { enemyPositions.Add(position); }
                }
                else if (RogueParty.Equals(self, target))
                {
                    partyMemberPositions.AddUnique(position, target);
                }
            }

            public void EndPredicate()
            {
                for (int i = 0; i < enemyPositions.Count; i++)
                {
                    partyMemberPositions.Remove(enemyPositions[i]);
                }
            }

            public void Dispose()
            {
                partyMemberPositions.Clear();
                predicators.Push(this);
            }
        }
    }
}
