using System.Collections.Generic;

namespace OchalikeSprites
{
    public interface ISortableBoneChildren<T>
    {
        IReadOnlyList<T> NormalFrontChildren { get; }

        IReadOnlyList<T> NormalRearChildren { get; }

        IReadOnlyList<T> BackFrontChildren { get; }

        IReadOnlyList<T> BackRearChildren { get; }
    }
}
