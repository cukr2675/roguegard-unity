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
    }
}
