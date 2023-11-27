using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class WoundedPartyMemberRogueMethodTarget : IRogueMethodTarget
    {
        public static WoundedPartyMemberRogueMethodTarget Instance { get; } = new WoundedPartyMemberRogueMethodTarget();

        public string Name => "傷ついた味方";
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        IRogueDetails IRogueDescription.Details => null;

        private static readonly Stack<Predicator> predicators = new Stack<Predicator>();

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
            public readonly List<Vector2Int> enemyPositions = new List<Vector2Int>();
            public readonly RoguePredicatorPositionList partyMemberPositions = new RoguePredicatorPositionList();

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
                else if (RogueParty.Equals(self, target) && target.Main.Stats.HP < StatsEffectedValues.GetMaxHP(target))
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
