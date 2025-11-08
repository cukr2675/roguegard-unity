namespace Lysionium
{
    public interface IMessageBoxSubview : ISubview
    {
        void DoScheduledAfterCompletion(ListMenuEventHandler onEndAnimation);
    }
}
