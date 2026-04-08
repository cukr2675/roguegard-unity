namespace Lysionium
{
    public interface IButtonViewItemHandlerBuilder<TItem, TMgr, TBuilder> : IViewItemHandlerBuilder<TItem, TMgr, TBuilder>
    {
        TBuilder OnClick(ClickItemHandler<TItem, TMgr> handler);
    }
}
