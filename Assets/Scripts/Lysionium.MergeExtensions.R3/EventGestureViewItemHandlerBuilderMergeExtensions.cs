using R3;

namespace Lysionium.MergeExtensions.R3
{
    public static class EventGestureViewItemHandlerBuilderMergeExtensions
    {
        public static TOut SubscribeEventGestureViewItemHandler<TItem, TMgr, TOut>(
            this IEventGestureViewItemHandlerBuilder<TItem, TMgr, TOut> builder,
            Subject<MergedViewItemHandleArg<TItem, TMgr, TOut, TItem>> subject)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (subject == null) throw new System.ArgumentNullException(nameof(subject));

            var onClickContext = new OnClickContext();
            builder.OnClick((item, manager) =>
            {
                lock (onClickContext)
                {
                    using var _ = onClickContext.OpenSelf();
                    subject.OnNext(new MergedViewItemHandleArg<TItem, TMgr, TOut, TItem>(item, manager, onClickContext));
                }
            });

            return (TOut)builder;
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> OnClick<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, SubmitItemHandler<TValue, TMgr> handler)
            where TBuilder : IEventGestureViewItemHandlerBuilder<TItem, TMgr, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (handler == null) throw new System.ArgumentNullException(nameof(handler));

            return source.ToBuilder().SubscribeTo<OnClickContext>((value, manager, _) =>
            {
                handler(value, manager);
            });
        }

        private class OnClickContext : MergedViewItemHandleContext { }
    }
}
