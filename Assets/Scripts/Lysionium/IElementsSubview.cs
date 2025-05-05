using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// 設定された <see cref="IElementHandler"/> とリストをもとに UI を表示するインターフェース。
    /// </summary>
    public interface IElementsSubview
    {
        void CommonInit();

        // リストとハンドラを Subpresenter でカプセル化すべきかもしれないが、
        // リストのコピーとハンドラのダウンキャストを考えると複雑になるためそのまま渡す
        void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubviewStateProvider stateProvider);

        void Show(HandleEndAnimation onEndAnimation = null);

        void Hide(bool back, HandleEndAnimation onEndAnimation = null);
    }
}
