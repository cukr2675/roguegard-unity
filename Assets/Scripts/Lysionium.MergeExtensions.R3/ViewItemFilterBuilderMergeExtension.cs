using R3;

namespace Lysionium.MergeExtensions.R3
{
    public static class ViewItemFilterBuilderMergeExtension
    {
        public static TOut SubscribeViewItemFilter<TItem, TMgr, TArg, TOut>(
            this IViewItemFilterBuilder<TItem, TMgr, TArg, TOut> builder, Subject<MergedViewItemHandleArg<TItem, TMgr, TArg, TOut, TItem>> subject)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (subject == null) throw new System.ArgumentNullException(nameof(subject));

            var filterContext = new FilterContext();
            builder.Filter((item, manager, arg) =>
            {
                lock (filterContext)
                {
                    using var _ = filterContext.OpenSelf();
                    subject.OnNext(new MergedViewItemHandleArg<TItem, TMgr, TArg, TOut, TItem>(item, manager, arg, filterContext));
                    if (filterContext.TryGetResult(out var result)) return result;
                    else return true;
                }
            });

            return (TOut)builder;
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> Filter<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, TMgr, TArg, bool> predicate)
            where TBuilder : IViewItemFilterBuilder<TItem, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (predicate == null) throw new System.ArgumentNullException(nameof(predicate));

            return source.ToBuilder().SubscribeTo<FilterContext>((value, manager, arg, context) =>
            {
                context.Result = predicate(value, manager, arg);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> Filter<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, bool> predicate)
            where TBuilder : IViewItemFilterBuilder<TItem, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (predicate == null) throw new System.ArgumentNullException(nameof(predicate));

            return source.ToBuilder().SubscribeTo<FilterContext>((value, manager, arg, context) =>
            {
                context.Result = predicate(value);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> Hide<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source)
            where TBuilder : IViewItemFilterBuilder<TItem, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));

            return source.ToBuilder().SubscribeTo<FilterContext>((value, manager, arg, context) =>
            {
                context.Result = false;
            });
        }

        private class FilterContext : MergedViewItemHandleContext<bool> { }
    }
}
