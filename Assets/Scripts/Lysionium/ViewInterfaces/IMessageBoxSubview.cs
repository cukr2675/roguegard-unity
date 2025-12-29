namespace Lysionium
{
    public interface IMessageBoxSubview : ISubview
    {
        bool IsInProgress { get; }

        void Clear();
        void Clear(IListuiManager manager, IListuiArg arg, ref ISubviewStateProvider stateProvider);

        void Append(string text);
        void Append(int integer);
        void Append(float number);

        void DoScheduledAfterCompletion(ListuiEventHandler onEndAnimation);
    }
}
