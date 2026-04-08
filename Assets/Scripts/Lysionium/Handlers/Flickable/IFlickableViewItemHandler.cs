namespace Lysionium
{
    public interface IFlickableViewItemHandler : IViewItemHandler
    {
        void KeyDown(object item, IListuiManager manager);
        void Expand(object item, IListuiManager manager);
        void KeyUp(object item, IListuiManager manager);
    }
}
