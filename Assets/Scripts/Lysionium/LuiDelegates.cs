namespace Lysionium
{
    public delegate void ClickItemHandler<TItem, TMgr, TArg>(TItem item, TMgr manager, TArg arg);

    public delegate void ClickItemHandler<TMgr, TArg>(TMgr manager, TArg arg);

    public delegate void LuiEventHandler(IListMenuManager manager, IListMenuArg arg);
}
