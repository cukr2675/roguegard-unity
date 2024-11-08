using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// 設定された <see cref="IElementHandler"/> とリストをもとに UI を表示するインターフェース。
    /// </summary>
    public interface IElementsSubView
    {
        void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider);

        void Show(HandleEndAnimation onEndAnimation = null);

        void Hide(bool back, HandleEndAnimation onEndAnimation = null);
    }
}
