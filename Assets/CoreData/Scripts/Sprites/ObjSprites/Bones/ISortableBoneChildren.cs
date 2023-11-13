using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface ISortableBoneChildren<T>
    {
        Spanning<T> NormalFrontChildren { get; }

        Spanning<T> NormalRearChildren { get; }

        Spanning<T> BackFrontChildren { get; }

        Spanning<T> BackRearChildren { get; }
    }
}
