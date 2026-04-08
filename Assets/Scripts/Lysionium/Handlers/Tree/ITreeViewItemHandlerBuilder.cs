using System.Collections.Generic;

namespace Lysionium
{
    public interface ITreeViewItemHandlerBuilder<TItem, TMgr, TBuilder>
    {
        TBuilder ChildrenFrom(System.Func<TItem, TMgr, IReadOnlyList<TItem>> selector);
    }
}
