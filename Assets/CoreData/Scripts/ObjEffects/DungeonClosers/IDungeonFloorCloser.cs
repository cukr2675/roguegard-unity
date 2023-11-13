using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IDungeonFloorCloser
    {
        bool Close(RogueObj self, bool exitDungeon);
    }
}
