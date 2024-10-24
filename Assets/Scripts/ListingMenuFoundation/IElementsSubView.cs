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
        /// <summary>
        /// このプロパティが true のとき <see cref="IListMenuManager"/> の動作を停止させる。アニメーションを待機させるために使用する
        /// </summary>
        bool HasManagerLock { get; }

        void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider);

        void Show(ElementsSubView.HandleEndAnimation handleEndAnimation = null);

        void Hide(bool back, ElementsSubView.HandleEndAnimation handleEndAnimation = null);
    }
}
