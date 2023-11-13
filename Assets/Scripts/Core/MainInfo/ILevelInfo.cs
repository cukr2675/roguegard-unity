using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface ILevelInfo
    {
        Spanning<int> NextTotalExps { get; }

        void Close(RogueObj self);

        void LevelUp(RogueObj self);

        void LevelDown(RogueObj self);
    }
}
