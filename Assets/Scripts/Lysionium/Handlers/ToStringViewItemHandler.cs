namespace Lysionium
{
    public class ToStringViewItemHandler : IViewItemHandler
    {
        public static ToStringViewItemHandler Instance { get; } = new();

        public string GetName(object element, IListMenuManager manager, IListMenuArg arg)
        {
            return element?.ToString();
        }

        string IViewItemHandler.GetStyle(object element, IListMenuManager manager, IListMenuArg arg)
        {
            return null;
        }
    }
}
