using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="IRogueMethodTarget"/> と <see cref="IRogueMethodRange"/> から有効対象を検索するためのインターフェース。
    /// </summary>
    public interface IRoguePredicator : System.IDisposable
    {
        Spanning<Vector2Int> Positions { get; }

        Spanning<RogueObj> GetObjs(Vector2Int position);

        void Predicate(RogueObj self, RogueObj target, Vector2Int position);

        void EndPredicate();
    }
}
