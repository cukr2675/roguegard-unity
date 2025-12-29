namespace Lysionium
{
    public interface IMessageBoxSubview : IListHandlerSubview
    {
        bool IsInProgress { get; }

        void Append(string text);
        void Append(int integer);
        void Append(float number);

        void Clear();

        void DoScheduledAfterCompletion(ListuiEventHandler onEndAnimation);
    }
}
