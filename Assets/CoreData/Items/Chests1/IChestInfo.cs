using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IChestInfo
    {
        IApplyRogueMethod BeOpened { get; }
        IApplyRogueMethod TakeIn { get; }
        IApplyRogueMethod PutOut { get; }
    }
}
