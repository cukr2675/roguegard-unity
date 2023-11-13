using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface ILevelInfoInitializer : ILevelInfo
    {
        void InitializeLv(RogueObj obj, int initialLv);
    }
}
