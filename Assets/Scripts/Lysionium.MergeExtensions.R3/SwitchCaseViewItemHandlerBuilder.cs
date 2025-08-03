using R3;

namespace Lysionium.MergeExtensions.R3
{
    public readonly struct SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue>
    {
        private readonly Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source;

        internal SwitchCaseViewItemHandlerBuilder(Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source)
        {
            this.source = source.Share();
        }

        public SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue> Case(
            System.Func<TValue, TMgr, TArg, bool> predicate, System.Action<Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>>> then)
        {
            then(source.Where(x => predicate(x.Value, x.Manager, x.Arg)));
            return new(source.Where(x => !predicate(x.Value, x.Manager, x.Arg)));
        }

        public SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue> Case(
            System.Func<TValue, bool> predicate, System.Action<Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>>> then)
        {
            then(source.Where(x => predicate(x.Value)));
            return new(source.Where(x => !predicate(x.Value)));
        }

        public void Otherwise(
            System.Action<Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>>> otherwise)
        {
            otherwise(source);
        }
    }
}
