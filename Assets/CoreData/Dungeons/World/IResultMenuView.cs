using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IResultMenuView : IElementsView
    {
        void SetResult(RogueObj player, RogueObj dungeon);
        void SetGameOver(RogueObj player, RogueObj dungeon);
    }
}
