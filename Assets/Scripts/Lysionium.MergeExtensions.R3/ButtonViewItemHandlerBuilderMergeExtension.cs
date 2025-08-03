using R3;

namespace Lysionium.MergeExtensions.R3
{
    public static class ButtonViewItemHandlerBuilderMergeExtension
    {
        public static TOut SubscribeButtonViewItemHandler<TItem, TMgr, TArg, TOut>(
            this IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, Subject<MergedViewItemHandleArg<TItem, TMgr, TArg, TOut, TItem>> subject)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (subject == null) throw new System.ArgumentNullException(nameof(subject));
            
            var onClickContext = new OnClickContext();
            builder.OnClick((item, manager, arg) =>
            {
                lock (onClickContext)
                {
                    using var _ = onClickContext.OpenSelf();
                    subject.OnNext(new MergedViewItemHandleArg<TItem, TMgr, TArg, TOut, TItem>(item, manager, arg, onClickContext));
                }
            });

            return (TOut)builder;
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> OnClick<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, ClickItemHandler<TValue, TMgr, TArg> handler)
            where TBuilder : IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (handler == null) throw new System.ArgumentNullException(nameof(handler));

            return source.ToBuilder().SubscribeTo<OnClickContext>((value, manager, arg, _) =>
            {
                handler(value, manager, arg);
            });
        }

        private class OnClickContext : MergedViewItemHandleContext { }
    }
}
