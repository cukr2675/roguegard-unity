namespace Lysionium
{
    public class FlickableOptionViewItemHandler<TMgr> : IFlickableViewItemHandler
    {
        public static FlickableOptionViewItemHandler<TMgr> Instance { get; } = new();

        public string GetName(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickableOption<TMgr>>(item, out var flickableOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetName(manager);

            return flickableOption.GetName(tMgr);
        }

        public string GetStyle(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickableOption<TMgr>>(item, out var flickableOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetStyle(manager);

            return flickableOption.GetStyle(tMgr);
        }

        public void KeyDown(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickableOption<TMgr>>(item, out var flickableOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            flickableOption.KeyDown(tMgr);
        }

        public void Expand(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickableOption<TMgr>>(item, out var flickableOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            flickableOption.Expand(tMgr);
        }

        public void KeyUp(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickableOption<TMgr>>(item, out var flickableOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            flickableOption.KeyUp(tMgr);
        }
    }
}
