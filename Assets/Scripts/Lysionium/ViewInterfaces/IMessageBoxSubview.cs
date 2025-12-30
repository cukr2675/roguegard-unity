using System.Text;

namespace Lysionium
{
    public interface IMessageBoxSubview : ISubview
    {
        bool IsInProgress { get; }

        void SetText(string text, IListuiManager manager, IListuiArg arg, ref ISubviewStateProvider stateProvider);
        void SetTextRaw(StringBuilder stringBuilder, IListuiManager manager, IListuiArg arg, ref ISubviewStateProvider stateProvider);

        void Append(string text);
        void AppendRaw(StringBuilder stringBuilder);
        void Clear();

        void DoScheduledAfterCompletion(ListuiEventHandler onEndAnimation);
    }
}
