using System.Collections.Generic;

namespace Lysionium
{
    public static class ViewItemHandlerBuilderExtensions
    {
        public static TOut NameFrom<TItem, TMgr, TArg, TOut>(
            this IViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, System.Func<TItem, string> selector)
        {
            return builder.NameFrom((item, _, _) => selector(item));
        }

        public static TOut StyleFrom<TItem, TMgr, TArg, TOut>(
            this IViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, System.Func<TItem, string> selector)
        {
            return builder.StyleFrom((item, _, _) => selector(item));
        }

        public static TOut Filter<TItem, TMgr, TArg, TOut>(
            this IViewItemFilterBuilder<TItem, TMgr, TArg, TOut> builder, System.Func<TItem, bool> predicate)
        {
            return builder.Filter((item, _, _) => predicate(item));
        }

        public static TOut ChildrenFrom<TItem, TMgr, TArg, TOut>(
            this ITreeViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, System.Func<TItem, IReadOnlyList<TItem>> selector)
        {
            return builder.ChildrenFrom((item, _, _) => selector(item));
        }

        public static TOut OnClick<TItem, TMgr, TArg, TOut>(
            this IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, System.Action<TItem> handler)
        {
            return builder.OnClick((item, _, _) => handler(item));
        }
    }
}
