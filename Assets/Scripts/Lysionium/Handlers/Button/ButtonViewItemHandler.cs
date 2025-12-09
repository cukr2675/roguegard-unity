namespace Lysionium
{
    public class ButtonViewItemHandler<TItem, TMgr, TArg> : IButtonViewItemHandler
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public System.Func<TItem, TMgr, TArg, string> GetName { get; set; }
        public System.Func<TItem, TMgr, TArg, string> GetStyle { get; set; }
        public ClickItemHandler<TItem, TMgr, TArg> Click { get; set; }

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
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr, TArg>)
            {
                return SelectOptionViewItemHandler<TMgr, TArg>.Instance.GetName(item, manager, arg);
            }

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetName(manager, arg);

            if (GetName != null) return GetName(tItem, tMgr, tArg);
            else return item?.ToString() ?? "null";
        }

        string IViewItemHandler.GetStyle(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr, TArg>)
            {
                return SelectOptionViewItemHandler<TMgr, TArg>.Instance.GetStyle(item, manager, arg);
            }

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetStyle(manager, arg);

            return GetStyle?.Invoke(tItem, tMgr, tArg) ?? string.Empty;
        }

        void IButtonViewItemHandler.Click(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr, TArg>)
            {
                SelectOptionViewItemHandler<TMgr, TArg>.Instance.Click(item, manager, arg);
                return;
            }

            if (Click == null) throw new System.InvalidOperationException($"{Click} が null です。");
            if (LuiAssert.Type<TItem>(item, out var tItem, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            Click(tItem, tMgr, tArg);
        }
    }
}
