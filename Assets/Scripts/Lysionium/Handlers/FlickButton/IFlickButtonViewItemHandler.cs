namespace Lysionium
{
    public interface IFlickButtonViewItemHandler : IViewItemHandler
    {
        void KeyDown(object item, IListuiManager manager);
        void Expand(object item, IListuiManager manager);
        void KeyUp(object item, IListuiManager manager);
    }
}
