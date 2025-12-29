using System.Collections.Generic;

namespace Lysionium
{
    public static class ListHandlerSubviewExtensions
    {
        public static void Show(
            this IListHandlerSubview subview, IReadOnlyList<object> list, IViewItemHandler handler,
            IListuiManager manager, IListuiArg arg, ref ISubviewStateProvider stateProvider,
            ListuiEventHandler onEndAnimation = null, ListuiEventHandler onHide = null)
        {
            subview.SetListHandler(list, handler, manager, arg, ref stateProvider);
            subview.Show(onEndAnimation, onHide);
        }

        public static void Show<TMgr, TArg>(
            this IListHandlerSubview subview, IReadOnlyList<ISelectOption<TMgr, TArg>> list,
            IListuiManager manager, IListuiArg arg, ref ISubviewStateProvider stateProvider,
            ListuiEventHandler onEndAnimation = null, ListuiEventHandler onHide = null)
        {
            subview.Show(list, SelectOptionViewItemHandler<TMgr, TArg>.Instance, manager, arg, ref stateProvider, onEndAnimation, onHide);
        }

        public static void Show<TMgr, TArg>(
            this IListHandlerSubview subview, IReadOnlyList<IKeyOption<TMgr, TArg>> list,
            IListuiManager manager, IListuiArg arg, ref ISubviewStateProvider stateProvider,
            ListuiEventHandler onEndAnimation = null, ListuiEventHandler onHide = null)
        {
            subview.Show(list, KeyOptionViewItemHandler<TMgr, TArg>.Instance, manager, arg, ref stateProvider, onEndAnimation, onHide);
        }
    }
}
