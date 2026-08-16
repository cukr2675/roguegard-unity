using System.Collections.Generic;
using System.Text;

namespace Lysionium
{
    public static class SubviewExtensions
    {
        public static void Show(
            this IListHandlerSubview subview, IReadOnlyList<object> list, IViewItemHandler handler,
            IListuiManager manager, ref ISubviewStateProvider stateProvider,
            System.Action<IListuiManager> onEndAnimation = null, System.Action<IListuiManager> onHide = null)
        {
            subview.SetListHandler(list, handler, manager, ref stateProvider);
            subview.Show(onEndAnimation, onHide);
        }

        public static void Show<TMgr>(
            this IListHandlerSubview subview, IReadOnlyList<ISelectOption<TMgr>> list,
            IListuiManager manager, ref ISubviewStateProvider stateProvider,
            System.Action<IListuiManager> onEndAnimation = null, System.Action<IListuiManager> onHide = null)
        {
            subview.Show(
                list, SelectOptionViewItemHandler<TMgr>.Instance, manager,
                ref stateProvider, onEndAnimation, onHide);
        }

        public static void Show<TMgr>(
            this IListHandlerSubview subview, IReadOnlyList<IKeyOption<TMgr>> list,
            IListuiManager manager, ref ISubviewStateProvider stateProvider,
            System.Action<IListuiManager> onEndAnimation = null, System.Action<IListuiManager> onHide = null)
        {
            subview.Show(
                list, KeyOptionViewItemHandler<TMgr>.Instance, manager,
                ref stateProvider, onEndAnimation, onHide);
        }

        public static void Show(
            this IMessageBoxSubview subview, string text,
            IListuiManager manager, ref ISubviewStateProvider stateProvider,
            System.Action<IListuiManager> onCompleted = null, System.Action<IListuiManager> onHide = null)
        {
            subview.SetText(text, manager, ref stateProvider);
            subview.Show(null, onHide);
            if (onCompleted != null) { subview.DoScheduledAfterCompletion(onCompleted); }
        }

        public static void ShowRaw(
            this IMessageBoxSubview subview, StringBuilder stringBuilder,
            IListuiManager manager, ref ISubviewStateProvider stateProvider,
            System.Action<IListuiManager> onCompleted = null, System.Action<IListuiManager> onHide = null)
        {
            subview.SetTextRaw(stringBuilder, manager, ref stateProvider);
            subview.Show(null, onHide);
            if (onCompleted != null) { subview.DoScheduledAfterCompletion(onCompleted); }
        }
    }
}
