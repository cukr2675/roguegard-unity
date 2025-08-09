namespace Lysionium
{
    public class ToStringViewItemHandler : IViewItemHandler
    {
        public static ToStringViewItemHandler Instance { get; } = new();

        public string GetName(object item, IListMenuManager manager, IListMenuArg arg)
        {
            return item?.ToString();
        }

        string IViewItemHandler.GetStyle(object item, IListMenuManager manager, IListMenuArg arg)
        {
            return string.Empty;
        }
    }
}
