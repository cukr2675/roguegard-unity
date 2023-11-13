using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public interface IAffectRogueMethod : IRogueMethod, IChangeStateRogueMethodCaller
    {
    }
}
