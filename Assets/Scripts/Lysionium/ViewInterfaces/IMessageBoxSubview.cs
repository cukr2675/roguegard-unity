namespace Lysionium
{
    public interface IMessageBoxSubview : IListHandlerSubview
    {
        void DoScheduledAfterCompletion(ListMenuEventHandler onEndAnimation);
    }
}
