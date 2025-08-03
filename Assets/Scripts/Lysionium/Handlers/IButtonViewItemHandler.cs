namespace Lysionium
{
    public interface IButtonViewItemHandler : IViewItemHandler
    {
        void Click(object item, IListMenuManager manager, IListMenuArg arg);
    }
}
