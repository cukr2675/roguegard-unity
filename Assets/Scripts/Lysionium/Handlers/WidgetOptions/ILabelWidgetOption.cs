namespace Lysionium
{
    public interface ILabelWidgetOption
    {
        string GetText(IListuiManager manager, IListuiArg arg);

        void ClickLink(string link, IListuiManager manager, IListuiArg arg);
    }
}
