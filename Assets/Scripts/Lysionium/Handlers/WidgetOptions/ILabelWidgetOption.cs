namespace Lysionium
{
    public interface ILabelWidgetOption
    {
        string GetText(IListMenuManager manager, IListMenuArg arg);

        void ClickLink(string link, IListMenuManager manager, IListMenuArg arg);
    }
}
