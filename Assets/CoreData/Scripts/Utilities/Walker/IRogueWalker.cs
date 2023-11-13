using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.RequireRelationalComponent]
    public interface IRogueWalker
    {
        Vector2Int GetWalk(RogueObj player, bool fear);
    }
}
