namespace Lysionium
{
    // 誤って使用することを避けるため、省略版は実装しない
    ///// <inheritdoc/>
    //public class SelectOptionViewItemHandler : SelectOptionViewItemHandler<IListMenuManager, IListMenuArg>
    //{
    //}
    ///// <inheritdoc/>
    //public class SelectOptionViewItemHandler<TMgr> : SelectOptionViewItemHandler<TMgr, IListMenuArg>
    //{
    //}

    /// <summary>
    /// モデルのリストではなく選択肢を扱いたいときに使用する <see cref="IViewItemHandler"/> 。
    /// </summary>
    public class SelectOptionViewItemHandler<TMgr, TArg> : IButtonViewItemHandler
    {
        public static SelectOptionViewItemHandler<TMgr, TArg> Instance { get; } = new();

        public string GetName(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<ISelectOption<TMgr, TArg>>(item, out var selectOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetName(manager, arg);

            return selectOption.GetName(tMgr, tArg);
        }

        public string GetStyle(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<ISelectOption<TMgr, TArg>>(item, out var selectOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetStyle(manager, arg);

            return selectOption.GetStyle(tMgr, tArg);
        }

        public void Click(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<ISelectOption<TMgr, TArg>>(item, out var selectOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            selectOption.Click(tMgr, tArg);
        }
    }
}
