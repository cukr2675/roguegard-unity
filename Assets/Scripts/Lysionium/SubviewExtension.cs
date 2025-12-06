using System.Collections.Generic;

namespace Lysionium
{
    public static class SubviewExtension
    {
        public static void Show(
            this IListHandlerSubview subview, IReadOnlyList<object> list, IViewItemHandler handler,
            IListMenuManager manager, IListMenuArg arg, ref ISubviewStateProvider stateProvider,
            ListMenuEventHandler onEndAnimation = null, ListMenuEventHandler onHide = null)
        {
            subview.SetParameters(list, handler, manager, arg, ref stateProvider);
            subview.Show(onEndAnimation, onHide);
        }
    }
}
