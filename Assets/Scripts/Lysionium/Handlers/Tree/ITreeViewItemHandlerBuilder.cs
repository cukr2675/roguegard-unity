using System.Collections.Generic;

namespace Lysionium
{
    public interface ITreeViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder>
    {
        TBuilder ChildrenFrom(System.Func<TItem, TMgr, TArg, IReadOnlyList<TItem>> selector);
    }
}
