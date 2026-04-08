namespace Lysionium
{
    public class ButtonViewItemHandler<TItem, TMgr> : IButtonViewItemHandler
        where TMgr : IListuiManager
    {
        public System.Func<TItem, TMgr, string> GetName { get; set; }
        public System.Func<TItem, TMgr, string> GetStyle { get; set; }
        public ClickItemHandler<TItem, TMgr> Click { get; set; }

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler{TMgr}"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption{TMgr}"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy { get; set; }

        public ButtonViewItemHandler(bool enableSelectOptionProxy = true)
        {
            EnableSelectOptionProxy = enableSelectOptionProxy;
        }

        string IViewItemHandler.GetName(object item, IListuiManager manager)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
            {
                return SelectOptionViewItemHandler<TMgr>.Instance.GetName(item, manager);
            }

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetName(manager);

            if (GetName != null) return GetName(tItem, tMgr);
            else return item?.ToString() ?? "null";
        }

        string IViewItemHandler.GetStyle(object item, IListuiManager manager)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
            {
                return SelectOptionViewItemHandler<TMgr>.Instance.GetStyle(item, manager);
            }

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetStyle(manager);

            return GetStyle?.Invoke(tItem, tMgr) ?? string.Empty;
        }

        void IButtonViewItemHandler.Click(object item, IListuiManager manager)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
            {
                SelectOptionViewItemHandler<TMgr>.Instance.Click(item, manager);
                return;
            }

            if (Click == null) throw new System.InvalidOperationException($"{Click} が null です。");
            if (LuiAssert.Type<TItem>(item, out var tItem, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            Click(tItem, tMgr);
        }
    }
}
