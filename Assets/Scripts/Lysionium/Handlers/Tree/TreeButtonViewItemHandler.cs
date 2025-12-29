using System.Collections.Generic;

namespace Lysionium
{
    public class TreeButtonViewItemHandler<TItem, TMgr, TArg> : IButtonViewItemHandler, ITreeViewItemHandler
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        public System.Func<TItem, TMgr, TArg, string> GetName { get; set; }
        public System.Func<TItem, TMgr, TArg, string> GetStyle { get; set; }
        public System.Func<TItem, TMgr, TArg, IReadOnlyList<TItem>> GetChildren { get; set; }
        public ClickItemHandler<TItem, TMgr, TArg> Click { get; set; }

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler{TMgr, TArg}"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption{TMgr, TArg}"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy { get; set; }

        public TreeButtonViewItemHandler(bool enableSelectOptionProxy = true)
        {
            EnableSelectOptionProxy = enableSelectOptionProxy;
        }

        string IViewItemHandler.GetName(object item, IListuiManager manager, IListuiArg arg)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr, TArg>)
                return SelectOptionViewItemHandler<TMgr, TArg>.Instance.GetName(item, manager, arg);
            if (EnableSelectOptionProxy && item is ITreeOption<TMgr, TArg>)
                return TreeOptionViewItemHandler<TMgr, TArg>.Instance.GetName(item, manager, arg);

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetName(manager, arg);

            if (GetName != null) return GetName(tItem, tMgr, tArg);
            else return item?.ToString() ?? "null";
        }

        string IViewItemHandler.GetStyle(object item, IListuiManager manager, IListuiArg arg)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr, TArg>)
                return SelectOptionViewItemHandler<TMgr, TArg>.Instance.GetStyle(item, manager, arg);
            if (EnableSelectOptionProxy && item is ITreeOption<TMgr, TArg>)
                return TreeOptionViewItemHandler<TMgr, TArg>.Instance.GetStyle(item, manager, arg);

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetStyle(manager, arg);

            return GetStyle?.Invoke(tItem, tMgr, tArg) ?? string.Empty;
        }

        IReadOnlyList<object> ITreeViewItemHandler.GetChildren(object item, IListuiManager manager, IListuiArg arg)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr, TArg>)
                return System.Array.Empty<object>();
            if (EnableSelectOptionProxy && item is ITreeOption<TMgr, TArg>)
                return TreeOptionViewItemHandler<TMgr, TArg>.Instance.GetChildren(item, manager, arg);

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return System.Array.Empty<object>();

            return (IReadOnlyList<object>)GetChildren?.Invoke(tItem, tMgr, tArg) ?? System.Array.Empty<object>();
        }

        void IButtonViewItemHandler.Click(object item, IListuiManager manager, IListuiArg arg)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr, TArg>)
            {
                SelectOptionViewItemHandler<TMgr, TArg>.Instance.Click(item, manager, arg);
                return;
            }
            if (EnableSelectOptionProxy && item is ITreeOption<TMgr, TArg>) throw new System.InvalidOperationException(
                $"{item} は {nameof(ITreeOption<TMgr, TArg>)} です。この型にクリックイベントは存在しません。");

            if (Click == null) throw new System.InvalidOperationException($"{Click} が null です。");
            if (LuiAssert.Type<TItem>(item, out var tItem, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            Click(tItem, tMgr, tArg);
        }
    }
}
