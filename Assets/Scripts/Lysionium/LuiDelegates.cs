namespace Lysionium
{
    public delegate void ClickItemHandler<TItem, TMgr, TArg>(TItem item, TMgr manager, TArg arg);

    public delegate void ClickItemHandler<TMgr, TArg>(TMgr manager, TArg arg);

    public delegate void ListMenuEventHandler(IListMenuManager manager, IListMenuArg arg);

    public delegate void ListMenuEventHandler<TMgr, TArg>(TMgr manager, TArg arg);
}
