using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// 敵を狙って発動する。（味方を巻き込む場合は発動しない）
    /// </summary>
    public class ForEnemyRogueMethodTarget : IRogueMethodTarget
    {
        public static ForEnemyRogueMethodTarget Instance { get; } = new ForEnemyRogueMethodTarget();

        public string Name => "敵";
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        object IRogueDescription.Details => null;

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
            public readonly RoguePredicatorPositionList enemyPositions = new RoguePredicatorPositionList();
            public readonly List<Vector2Int> partyMemberPositions = new List<Vector2Int>();

            public Spanning<Vector2Int> Positions => enemyPositions.Positions;

            public Spanning<RogueObj> GetObjs(Vector2Int position)
            {
                return enemyPositions.GetObjs(position);
            }

            public void Predicate(RogueObj self, RogueObj target, Vector2Int position)
            {
                if (StatsEffectedValues.AreVS(self, target))
                {
                    enemyPositions.AddUnique(position, target);
                }
                else if (RogueParty.Equals(self, target))
                {
                    if (!partyMemberPositions.Contains(position)) { partyMemberPositions.Add(position); }
                }
            }

            public void EndPredicate()
            {
                for (int i = 0; i < partyMemberPositions.Count; i++)
                {
                    enemyPositions.Remove(partyMemberPositions[i]);
                }
            }

            public void Dispose()
            {
                enemyPositions.Clear();
                predicators.Push(this);
            }
        }
    }
}
