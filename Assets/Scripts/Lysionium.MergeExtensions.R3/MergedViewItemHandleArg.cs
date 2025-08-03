namespace Lysionium.MergeExtensions.R3
{
    public readonly struct MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>
    {
        public TValue Value { get; }
        public TMgr Manager { get; }
        public TArg Arg { get; }
        internal MergedViewItemHandleContext Context { get; }

        /// <summary>
        /// <see cref="Value"/> のエイリアス
        /// </summary>
        public TValue Value_ => Value;

        /// <summary>
        /// <see cref="Manager"/> のエイリアス
        /// </summary>
        public TValue Manager_ => Value;

        /// <summary>
        /// <see cref="Arg"/> のエイリアス
        /// </summary>
        public TValue Arg_ => Value;

        public MergedViewItemHandleArg(TValue value, TMgr manager, TArg arg, MergedViewItemHandleContext context)
        {
            Value = value;
            Manager = manager;
            Arg = arg;
            Context = context;
        }

        public MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, T> SetValue<T>(T value)
        {
            return new MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, T>(value, Manager, Arg, Context);
        }
    }
}
