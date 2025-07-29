namespace Lysionium
{
    public class ButtonViewItemHandler<TItem, TMgr, TArg> : IButtonViewItemHandler
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public ItemNameSelector<TItem, TMgr, TArg> GetName { get; set; }
        public ItemStyleSelector<TItem, TMgr, TArg> GetStyle { get; set; }
        public ClickItemHandler<TItem, TMgr, TArg> HandleClick { get; set; }

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy { get; set; }

        public ButtonViewItemHandler(bool enableSelectOptionProxy = true)
        {
            EnableSelectOptionProxy = enableSelectOptionProxy;
        }

        string IViewItemHandler.GetName(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (EnableSelectOptionProxy && item is ISelectOption) { return SelectOptionViewItemHandler.Instance.GetName(item, manager, arg); }

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetName(manager, arg);

            if (GetName != null) return GetName(tItem, tMgr, tArg);
            else return item?.ToString() ?? "null";
        }

        string IViewItemHandler.GetStyle(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (EnableSelectOptionProxy && item is ISelectOption) { return SelectOptionViewItemHandler.Instance.GetStyle(item, manager, arg); }

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetStyle(manager, arg);

            return GetStyle?.Invoke(tItem, tMgr, tArg);
        }

        void IButtonViewItemHandler.HandleClick(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (EnableSelectOptionProxy && item is ISelectOption)
            {
                SelectOptionViewItemHandler.Instance.HandleClick(item, manager, arg);
                return;
            }

            if (HandleClick == null) throw new System.InvalidOperationException($"{HandleClick} が null です。");
            if (LuiAssert.Type<TItem>(item, out var tItem, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            HandleClick(tItem, tMgr, tArg);
        }
    }
}
