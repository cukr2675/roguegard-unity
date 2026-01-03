namespace Lysionium
{
    public interface IFlickableViewItemHandler : IViewItemHandler
    {
        void KeyDown(object item, IListuiManager manager, IListuiArg arg);
        void Expand(object item, IListuiManager manager, IListuiArg arg);
        void KeyUp(object item, IListuiManager manager, IListuiArg arg);
    }
}
