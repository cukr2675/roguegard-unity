using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RoguePredicatorPositionList
    {
        private readonly Dictionary<Vector2Int, RogueObjList> table = new();
        private readonly List<Vector2Int> _positions = new();

        public Spanning<Vector2Int> Positions => Spanning.Get(_positions);

        private static readonly Stack<RogueObjList> pool = new();

        public Spanning<RogueObj> GetObjs(Vector2Int position)
        {
            if (table.TryGetValue(position, out var objs)) return objs.Span;
            else return Spanning<RogueObj>.Empty;
        }

        public void AddUnique(Vector2Int position, RogueObj obj)
        {
            if (!table.TryGetValue(position, out var list))
            {
                if (!pool.TryPop(out list)) { list = new RogueObjList(); }
                table.Add(position, list);
                _positions.Add(position);
            }

            list.TryAddUnique(obj);
        }

        public bool Remove(Vector2Int position)
        {
            if (!_positions.Remove(position)) return false;

            var list = table[position];
            list.Clear();
            pool.Push(list);
            table.Remove(position);
            return true;
        }

        public void Clear()
        {
            foreach (var pair in table)
            {
                pair.Value.Clear();
                pool.Push(pair.Value);
            }
            table.Clear();
            _positions.Clear();
        }
    }
}
