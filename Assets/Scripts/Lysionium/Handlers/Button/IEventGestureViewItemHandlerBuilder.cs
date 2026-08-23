namespace Lysionium
{
    public interface IEventGestureViewItemHandlerBuilder<TItem, TMgr, TBuilder> :
        IViewItemHandlerBuilder<TItem, TMgr, TBuilder>
    {
        TBuilder OnEventGestureConfirmed(string eventGestureName, SubmitItemHandler<TItem, TMgr> handler);
    }
}
