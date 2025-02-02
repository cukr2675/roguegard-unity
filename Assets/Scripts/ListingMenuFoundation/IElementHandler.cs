using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// <see cref="IElementsSubView"/> に各要素をどのように扱わせるかを設定するインターフェース。
    /// 一つのメニュー画面が複数の <see cref="IElementHandler"/> を持つ可能性があるため分けて考える。
    /// </summary>
    public interface IElementHandler
    {
        string GetName(object element, IListMenuManager manager, IListMenuArg arg);

        // Roguegard の実装を見ると、アイコン以外の情報も同時に返すほうが効率的なため使用していない
        // GetName はデバッグにも使用できるが GetIcon は不向き
        //Sprite GetIcon(object element, IListMenuManager manager, IListMenuArg arg);

        string GetStyle(object element, IListMenuManager manager, IListMenuArg arg);
    }
}
