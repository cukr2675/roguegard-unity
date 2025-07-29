namespace Lysionium
{
    public interface IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder> : IViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder>
    {
        TBuilder OnClick(ClickItemHandler<TItem, TMgr, TArg> handler);
    }
}
