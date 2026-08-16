namespace Lysionium
{
    public class FlickButtonOptionViewItemHandler<TMgr> : IFlickButtonViewItemHandler
    {
        public static FlickButtonOptionViewItemHandler<TMgr> Instance { get; } = new();

        public string GetName(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickButtonOption<TMgr>>(item, out var flickButtonOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetName(manager);

            return flickButtonOption.GetName(tMgr);
        }

        public string GetStyle(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickButtonOption<TMgr>>(item, out var flickButtonOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetStyle(manager);

            return flickButtonOption.GetStyle(tMgr);
        }

        public void KeyDown(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickButtonOption<TMgr>>(item, out var flickButtonOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            flickButtonOption.KeyDown(tMgr);
        }

        public void Expand(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickButtonOption<TMgr>>(item, out var flickButtonOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            flickButtonOption.Expand(tMgr);
        }

        public void KeyUp(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickButtonOption<TMgr>>(item, out var flickButtonOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            flickButtonOption.KeyUp(tMgr);
        }
    }
}
