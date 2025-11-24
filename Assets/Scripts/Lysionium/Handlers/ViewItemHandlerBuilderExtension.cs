namespace Lysionium
{
    public static class ViewItemHandlerBuilderExtension
    {
        public static TOut NameFrom<TItem, TMgr, TArg, TOut>(this IViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, System.Func<TItem, string> selector)
        {
            return builder.NameFrom((item, _, _) => selector(item));
        }

        public static TOut StyleFrom<TItem, TMgr, TArg, TOut>(this IViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, System.Func<TItem, string> selector)
        {
            return builder.StyleFrom((item, _, _) => selector(item));
        }

        public static TOut Filter<TItem, TMgr, TArg, TOut>(this IViewItemFilterBuilder<TItem, TMgr, TArg, TOut> builder, System.Func<TItem, bool> predicate)
        {
            return builder.Filter((item, _, _) => predicate(item));
        }

        public static TOut OnClick<TItem, TMgr, TArg, TOut>(
            this IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, System.Action<TItem> handler)
        {
            return builder.OnClick((item, manager, arg) => handler(item));
        }

        public static TOut OnClick<TItem, TMgr, TArg, TOut>(
            this IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, System.Func<TItem, ClickItemHandler<TMgr, TArg>> selector)
        {
            return builder.OnClick((item, manager, arg) => selector(item)?.Invoke(manager, arg));
        }

        public static TOut OnClick<TItem, TMgr, TArg, TOut>(
            this IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, System.Func<TItem, ClickItemHandler<TItem, TMgr, TArg>> selector)
        {
            return builder.OnClick((item, manager, arg) => selector(item)?.Invoke(item, manager, arg));
        }
    }
}
