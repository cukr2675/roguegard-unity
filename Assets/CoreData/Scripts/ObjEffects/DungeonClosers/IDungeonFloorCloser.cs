using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IDungeonFloorCloser
    {
        void RemoveClose(RogueObj self, bool exitDungeon);
    }
}
