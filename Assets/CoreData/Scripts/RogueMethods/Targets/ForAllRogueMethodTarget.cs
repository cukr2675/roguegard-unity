using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// 敵味方問わず全員に効果がある。
    /// </summary>
    public class ForAllRogueMethodTarget : IRogueMethodTarget
    {
        public static ForAllRogueMethodTarget Instance { get; } = new ForAllRogueMethodTarget();

        public string Name => "全員";
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

            predicator._positions.Clear();
            return predicator;
        }

        private class Predicator : IRoguePredicator
        {
            public readonly RoguePredicatorPositionList _positions = new RoguePredicatorPositionList();

            public Spanning<Vector2Int> Positions => _positions.Positions;

            public Spanning<RogueObj> GetObjs(Vector2Int position)
            {
                return _positions.GetObjs(position);
            }

            public void Predicate(RogueObj self, RogueObj target, Vector2Int position)
            {
                _positions.AddUnique(position, target);
            }

            public void EndPredicate()
            {
            }

            public void Dispose()
            {
                _positions.Clear();
                predicators.Push(this);
            }
        }
    }
}
