using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IEatActiveRogueMethod : IActiveRogueMethod, IRogueDescription
    {
        Spanning<IKeyword> Edibles { get; }
    }
}
