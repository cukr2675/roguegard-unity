namespace Lysionium.MergeExtensions.R3
{
    public readonly struct MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>
    {
        public TValue Value { get; }
        public TMgr Manager { get; }
        internal MergedViewItemHandleContext Context { get; }

        /// <summary>
        /// <see cref="Value"/> のエイリアス
        /// </summary>
        public TValue Value_ => Value;

        /// <summary>
        /// <see cref="Manager"/> のエイリアス
        /// </summary>
        public TMgr Manager_ => Manager;

        public MergedViewItemHandleArg(TValue value, TMgr manager, MergedViewItemHandleContext context)
        {
            Value = value;
            Manager = manager;
            Context = context;
        }

        public MergedViewItemHandleArg<TItem, TMgr, TBuilder, T> SetValue<T>(T value)
        {
            return new MergedViewItemHandleArg<TItem, TMgr, TBuilder, T>(value, Manager, Context);
        }
    }
}
