using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// リスト処理プレゼンタ。
    /// <see cref="IElementsView"/> に各要素をどのように扱わせるかを設定するインターフェース。
    /// 一つの <see cref="IListMenu"/> が複数の <see cref="IElementPresenter"/> を持つ可能性があるため分けて考える。
    /// </summary>
    [System.Obsolete]
    public interface IElementPresenter
    {
        string GetItemName(object element, object manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void ActivateItem(object element, object manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);
    }
}
