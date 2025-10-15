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
        public TMgr Manager_ => Manager;

        /// <summary>
        /// <see cref="Arg"/> のエイリアス
        /// </summary>
        public TArg Arg_ => Arg;

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
