using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IPathBuilder
    {
        bool UpdatePath(RogueObj self, Vector2Int targetPosition);

        bool TryGetNextDirection(RogueObj self, out RogueDirection nextDirection);
    }
}
