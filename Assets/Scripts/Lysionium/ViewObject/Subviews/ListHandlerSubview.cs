using System.Collections.Generic;

namespace Lysionium.Views
{
    public abstract class ListHandlerSubview : Subview, IListHandlerSubview
    {
        public abstract void SetListHandler(
            IReadOnlyList<object> list, IViewItemHandler handler, IListuiManager manager, ref ISubviewStateProvider stateProvider);
    }
}
