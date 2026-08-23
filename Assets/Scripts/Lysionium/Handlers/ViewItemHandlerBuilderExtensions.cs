using System.Collections.Generic;

namespace Lysionium
{
    public static class ViewItemHandlerBuilderExtensions
    {
        public static TOut NameFrom<TItem, TMgr, TOut>(
            this IViewItemHandlerBuilder<TItem, TMgr, TOut> builder, System.Func<TItem, string> selector)
        {
            return builder.NameFrom((item, _) => selector(item));
        }

        public static TOut StyleFrom<TItem, TMgr, TOut>(
            this IViewItemHandlerBuilder<TItem, TMgr, TOut> builder, System.Func<TItem, string> selector)
        {
            return builder.StyleFrom((item, _) => selector(item));
        }

        public static TOut Filter<TItem, TMgr, TOut>(
            this IViewItemFilterBuilder<TItem, TMgr, TOut> builder, System.Func<TItem, bool> predicate)
        {
            return builder.Filter((item, _) => predicate(item));
        }

        public static TOut ChildrenFrom<TItem, TMgr, TOut>(
            this ITreeViewItemHandlerBuilder<TItem, TMgr, TOut> builder, System.Func<TItem, IReadOnlyList<TItem>> selector)
        {
            return builder.ChildrenFrom((item, _) => selector(item));
        }

        public static TOut OnClick<TItem, TMgr, TOut>(
            this IEventGestureViewItemHandlerBuilder<TItem, TMgr, TOut> builder, System.Action<TItem> handler)
        {
            return builder.OnEventGestureConfirmed("Click", (item, _) => handler(item));
        }

        public static TOut OnClick<TItem, TMgr, TOut>(
            this IEventGestureViewItemHandlerBuilder<TItem, TMgr, TOut> builder, SubmitItemHandler<TItem, TMgr> handler)
        {
            return builder.OnEventGestureConfirmed("Click", (item, manager) => handler(item, manager));
        }
    }
}
