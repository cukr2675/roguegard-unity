namespace Lysionium
{
    public delegate string ItemNameSelector<TItem, TMgr, TArg>(TItem item, TMgr manager, TArg arg);

    public delegate string ItemNameSelector<TMgr, TArg>(TMgr manager, TArg arg);

    public delegate string ItemStyleSelector<TItem, TMgr, TArg>(TItem item, TMgr manager, TArg arg);

    public delegate string ItemStyleSelector<TMgr, TArg>(TMgr manager, TArg arg);

    public delegate void ClickItemHandler<TItem, TMgr, TArg>(TItem item, TMgr manager, TArg arg);

    public delegate void ClickItemHandler<TMgr, TArg>(TMgr manager, TArg arg);

    public delegate void EndAnimationHandler(IListMenuManager manager, IListMenuArg arg);
}
