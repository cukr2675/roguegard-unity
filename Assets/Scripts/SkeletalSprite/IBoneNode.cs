using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public interface IBoneNode
    {
        IBone Bone { get; }

        IReadOnlyList<IBoneNode> Children { get; }
    }
}
