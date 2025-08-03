using R3;

namespace Lysionium.MergeExtensions.R3
{
    // SubscribeTo の型引数入力で楽をするための構造体
    public readonly struct MergedViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue>
    {
        private readonly Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source;

        internal MergedViewItemHandlerBuilder(Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source)
        {
            this.source = source.Share();
        }

        public MergedViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue> SubscribeTo<TCtx>(System.Action<TValue, TMgr, TArg, TCtx> action)
            where TCtx : MergedViewItemHandleContext
        {
            // 購読破棄は ViewData.Dispose で行う
            source.Subscribe(x =>
            {
                if (x.Context is TCtx context)
                {
                    action(x.Value, x.Manager, x.Arg, context);
                }
            });
            return this;
        }

        public Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> ToObservable() => source;

        public static implicit operator Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>>(
            MergedViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue> builder)
        {
            return builder.source;
        }
    }
}
