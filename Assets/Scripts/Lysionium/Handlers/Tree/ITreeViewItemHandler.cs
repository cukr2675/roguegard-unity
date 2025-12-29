using System.Collections.Generic;

namespace Lysionium
{
    // 設計メモ: IButtonViewItemHandler 以外とも組み合わせられるように IViewItemHandler の派生インターフェースにする
    public interface ITreeViewItemHandler : IViewItemHandler
    {
        IReadOnlyList<object> GetChildren(object item, IListMenuManager manager, IListMenuArg arg);
    }
}
