namespace Lysionium
{
    public class FlickableOptionViewItemHandler<TMgr, TArg> : IFlickableViewItemHandler
    {
        public static FlickableOptionViewItemHandler<TMgr, TArg> Instance { get; } = new();

        public string GetName(object item, IListuiManager manager, IListuiArg arg)
        {
            if (LuiAssert.Type<IFlickableOption<TMgr, TArg>>(item, out var flickableOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetName(manager, arg);

            return flickableOption.GetName(tMgr, tArg);
        }

        public string GetStyle(object item, IListuiManager manager, IListuiArg arg)
        {
            if (LuiAssert.Type<IFlickableOption<TMgr, TArg>>(item, out var flickableOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetStyle(manager, arg);

            return flickableOption.GetStyle(tMgr, tArg);
        }

        public void KeyDown(object item, IListuiManager manager, IListuiArg arg)
        {
            if (LuiAssert.Type<IFlickableOption<TMgr, TArg>>(item, out var flickableOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            flickableOption.KeyDown(tMgr, tArg);
        }

        public void Expand(object item, IListuiManager manager, IListuiArg arg)
        {
            if (LuiAssert.Type<IFlickableOption<TMgr, TArg>>(item, out var flickableOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            flickableOption.Expand(tMgr, tArg);
        }

        public void KeyUp(object item, IListuiManager manager, IListuiArg arg)
        {
            if (LuiAssert.Type<IFlickableOption<TMgr, TArg>>(item, out var flickableOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            flickableOption.KeyUp(tMgr, tArg);
        }
    }
}
