namespace Lysionium
{
    public interface ILabelWidgetOption
    {
        string GetText(IListuiManager manager);

        void ClickLink(string link, IListuiManager manager);
    }
}
