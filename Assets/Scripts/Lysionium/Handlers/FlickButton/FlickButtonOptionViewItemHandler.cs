namespace Lysionium
{
    public class FlickButtonOptionViewItemHandler<TMgr> : SelectOptionViewItemHandler<TMgr>
    {
        public static new FlickButtonOptionViewItemHandler<TMgr> Instance { get; } = new();

        public void Press(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickButtonOption<TMgr>>(item, out var flickButtonOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            flickButtonOption.Press(tMgr);
        }

        public void Expand(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickButtonOption<TMgr>>(item, out var flickButtonOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            flickButtonOption.Expand(tMgr);
        }

        public void Release(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IFlickButtonOption<TMgr>>(item, out var flickButtonOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            flickButtonOption.Release(tMgr);
        }
    }
}
