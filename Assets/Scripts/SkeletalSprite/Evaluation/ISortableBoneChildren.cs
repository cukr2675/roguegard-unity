using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public interface ISortableBoneChildren<T>
    {
        IReadOnlyList<T> NormalFrontChildren { get; }

        IReadOnlyList<T> NormalRearChildren { get; }

        IReadOnlyList<T> BackFrontChildren { get; }

        IReadOnlyList<T> BackRearChildren { get; }
    }
}
