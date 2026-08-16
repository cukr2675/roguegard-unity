using System.Collections.Generic;

namespace Lysionium
{
    public class TreeButtonViewItemHandler<TItem, TMgr> : IButtonViewItemHandler, ITreeViewItemHandler
        where TMgr : IListuiManager
    {
        public System.Func<TItem, TMgr, string> GetName { get; set; }
        public System.Func<TItem, TMgr, string> GetStyle { get; set; }
        public System.Func<TItem, TMgr, IReadOnlyList<TItem>> GetChildren { get; set; }
        public ClickItemHandler<TItem, TMgr> Click { get; set; }

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler{TMgr}"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption{TMgr}"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy { get; set; }

        private static readonly IReadOnlyList<string> clickSingle = new List<string>() { "Click" };

        public TreeButtonViewItemHandler(bool enableSelectOptionProxy = true)
        {
            EnableSelectOptionProxy = enableSelectOptionProxy;
        }

        string IViewItemHandler.GetName(object item, IListuiManager manager)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
                return SelectOptionViewItemHandler<TMgr>.Instance.GetName(item, manager);
            if (EnableSelectOptionProxy && item is ITreeOption<TMgr>)
                return TreeOptionViewItemHandler<TMgr>.Instance.GetName(item, manager);

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetName(manager);

            if (GetName != null) return GetName(tItem, tMgr);
            else return item?.ToString() ?? "null";
        }

        string IViewItemHandler.GetStyle(object item, IListuiManager manager)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
                return SelectOptionViewItemHandler<TMgr>.Instance.GetStyle(item, manager);
            if (EnableSelectOptionProxy && item is ITreeOption<TMgr>)
                return TreeOptionViewItemHandler<TMgr>.Instance.GetStyle(item, manager);

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetStyle(manager);

            return GetStyle?.Invoke(tItem, tMgr) ?? string.Empty;
        }

        IReadOnlyList<object> ITreeViewItemHandler.GetChildren(object item, IListuiManager manager)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
                return System.Array.Empty<object>();
            if (EnableSelectOptionProxy && item is ITreeOption<TMgr>)
                return TreeOptionViewItemHandler<TMgr>.Instance.GetChildren(item, manager);

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return System.Array.Empty<object>();

            return (IReadOnlyList<object>)GetChildren?.Invoke(tItem, tMgr) ?? System.Array.Empty<object>();
        }

        IReadOnlyList<string> IButtonViewItemHandler.GetCandidateClickNames(object item, IListuiManager manager)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
                return System.Array.Empty<string>();
            if (EnableSelectOptionProxy && item is ITreeOption<TMgr>) throw new System.InvalidOperationException(
                $"{item} は {nameof(ITreeOption<TMgr>)} です。この型にクリックイベントは存在しません。");

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return System.Array.Empty<string>();

            return clickSingle;
        }

        void IButtonViewItemHandler.Click(object item, IListuiManager manager, string clickName)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
            {
                SelectOptionViewItemHandler<TMgr>.Instance.Click(item, manager, clickName);
                return;
            }
            if (EnableSelectOptionProxy && item is ITreeOption<TMgr>) throw new System.InvalidOperationException(
                $"{item} は {nameof(ITreeOption<TMgr>)} です。この型にクリックイベントは存在しません。");

            if (Click == null) throw new System.InvalidOperationException($"{Click} が null です。");
            if (LuiAssert.Type<TItem>(item, out var tItem, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            Click(tItem, tMgr, clickName);
        }
    }
}
