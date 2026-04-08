using R3;

namespace Lysionium.MergeExtensions.R3
{
    public static class ViewItemFilterBuilderMergeExtensions
    {
        public static TOut SubscribeViewItemFilter<TItem, TMgr, TOut>(
            this IViewItemFilterBuilder<TItem, TMgr, TOut> builder, Subject<MergedViewItemHandleArg<TItem, TMgr, TOut, TItem>> subject)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (subject == null) throw new System.ArgumentNullException(nameof(subject));

            var filterContext = new FilterContext();
            builder.Filter((item, manager) =>
            {
                lock (filterContext)
                {
                    using var _ = filterContext.OpenSelf();
                    subject.OnNext(new MergedViewItemHandleArg<TItem, TMgr, TOut, TItem>(item, manager, filterContext));
                    if (filterContext.TryGetResult(out var result)) return result;
                    else return true;
                }
            });

            return (TOut)builder;
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> Filter<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, System.Func<TValue, TMgr, bool> predicate)
            where TBuilder : IViewItemFilterBuilder<TItem, TMgr, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (predicate == null) throw new System.ArgumentNullException(nameof(predicate));

            return source.ToBuilder().SubscribeTo<FilterContext>((value, manager, context) =>
            {
                context.Result = predicate(value, manager);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> Filter<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, System.Func<TValue, bool> predicate)
            where TBuilder : IViewItemFilterBuilder<TItem, TMgr, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (predicate == null) throw new System.ArgumentNullException(nameof(predicate));

            return source.ToBuilder().SubscribeTo<FilterContext>((value, manager, context) =>
            {
                context.Result = predicate(value);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> Hide<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source)
            where TBuilder : IViewItemFilterBuilder<TItem, TMgr, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));

            return source.ToBuilder().SubscribeTo<FilterContext>((value, manager, context) =>
            {
                context.Result = false;
            });
        }

        private class FilterContext : MergedViewItemHandleContext<bool> { }
    }
}
