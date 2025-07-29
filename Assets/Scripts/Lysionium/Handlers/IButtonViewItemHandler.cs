namespace Lysionium
{
    public interface IButtonViewItemHandler : IViewItemHandler
    {
        void HandleClick(object item, IListMenuManager manager, IListMenuArg arg);
    }
}
