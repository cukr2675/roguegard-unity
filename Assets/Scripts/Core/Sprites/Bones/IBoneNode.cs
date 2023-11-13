using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IBoneNode
    {
        IBone Bone { get; }

        Spanning<IBoneNode> Children { get; }
    }
}
