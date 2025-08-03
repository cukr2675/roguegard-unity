namespace Lysionium
{
    /// <summary>
    /// モデルのリストではなく選択肢を扱いたいときに使用する <see cref="IViewItemHandler"/> 。
    /// </summary>
    public class SelectOptionViewItemHandler : IButtonViewItemHandler
    {
        public static SelectOptionViewItemHandler Instance { get; } = new SelectOptionViewItemHandler();

        public string GetName(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<ISelectOption>(item, out var selectOption)) return string.Empty;

            return selectOption.GetName(manager, arg);
        }

        public string GetStyle(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<ISelectOption>(item, out var selectOption)) return string.Empty;

            return selectOption.GetStyle(manager, arg);
        }

        public void Click(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<ISelectOption>(item, out var selectOption, manager)) return;

            selectOption.Click(manager, arg);
        }
    }
}
