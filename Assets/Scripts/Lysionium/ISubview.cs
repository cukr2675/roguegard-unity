using System.Collections.Generic;

namespace Lysionium
{
    /// <summary>
    /// 設定された <see cref="IViewItemHandler"/> とリストをもとに UI を表示するインターフェース。
    /// </summary>
    public interface ISubview
    {
        void CommonInit();

        // リストとハンドラを Subpresenter でカプセル化すべきかもしれないが、
        // リストのコピーとハンドラのダウンキャストを考えると複雑になるためそのまま渡す
        void SetParameters(
            IReadOnlyList<object> list, IViewItemHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref ISubviewStateProvider stateProvider);

        void Show(ListMenuEventHandler onEndAnimation = null, ListMenuEventHandler onHide = null);

        void Hide(bool back, ListMenuEventHandler onEndAnimation = null);
    }
}
