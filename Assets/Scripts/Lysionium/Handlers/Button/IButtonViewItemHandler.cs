namespace Lysionium
{
    public interface IButtonViewItemHandler : IViewItemHandler
    {
        void Click(object item, IListuiManager manager, IListuiArg arg);
    }
}
