namespace Lysionium
{
    public class ToStringViewItemHandler : IViewItemHandler
    {
        public static ToStringViewItemHandler Instance { get; } = new();

        public string GetName(object item, IListuiManager manager, IListuiArg arg)
        {
            return item?.ToString();
        }

        string IViewItemHandler.GetStyle(object item, IListuiManager manager, IListuiArg arg)
        {
            return string.Empty;
        }
    }
}
