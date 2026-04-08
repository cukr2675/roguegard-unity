using R3;

namespace Lysionium.MergeExtensions.R3
{
    public readonly struct SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TBuilder, TValue>
    {
        private readonly Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source;

        internal SwitchCaseViewItemHandlerBuilder(Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source)
        {
            this.source = source.Share();
        }

        public SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TBuilder, TValue> Case(
            System.Func<TValue, TMgr, bool> predicate, System.Action<Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>>> then)
        {
            then(source.Where(x => predicate(x.Value, x.Manager)));
            return new(source.Where(x => !predicate(x.Value, x.Manager)));
        }

        public SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TBuilder, TValue> Case(
            System.Func<TValue, bool> predicate, System.Action<Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>>> then)
        {
            then(source.Where(x => predicate(x.Value)));
            return new(source.Where(x => !predicate(x.Value)));
        }

        public void Otherwise(
            System.Action<Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>>> otherwise)
        {
            otherwise(source);
        }
    }
}
