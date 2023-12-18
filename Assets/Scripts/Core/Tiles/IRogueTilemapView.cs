using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IRogueTilemapView
    {
        Vector2Int Size { get; }

        Spanning<RogueObj> VisibleObjs { get; }

        void GetTile(Vector2Int position, out bool visible, out IRogueTile tile, out RogueObj tileObj);
    }
}
