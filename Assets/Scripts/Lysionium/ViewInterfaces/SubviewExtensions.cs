using System.Collections.Generic;
using System.Text;

namespace Lysionium
{
    public static class SubviewExtensions
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

        public static void Show(
            this IMessageBoxSubview subview, string text,
            IListuiManager manager, IListuiArg arg, ref ISubviewStateProvider stateProvider,
            ListuiEventHandler onCompleted = null, ListuiEventHandler onHide = null)
        {
            subview.SetText(text, manager, arg, ref stateProvider);
            subview.Show(null, onHide);
            if (onCompleted != null) { subview.DoScheduledAfterCompletion(onCompleted); }
        }

        public static void ShowRaw(
            this IMessageBoxSubview subview, StringBuilder stringBuilder,
            IListuiManager manager, IListuiArg arg, ref ISubviewStateProvider stateProvider,
            ListuiEventHandler onCompleted = null, ListuiEventHandler onHide = null)
        {
            subview.SetTextRaw(stringBuilder, manager, arg, ref stateProvider);
            subview.Show(null, onHide);
            if (onCompleted != null) { subview.DoScheduledAfterCompletion(onCompleted); }
        }
    }
}
