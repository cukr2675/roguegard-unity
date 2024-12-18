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

        // アイコンを返す形式にするとラムダ式の記述が面倒になってしまう
        // （ラムダ式で Name Icon Style をまとめて返す場合はタプルを使う）
        //string GetName(object element, IListMenuManager manager, IListMenuArg arg, ref Sprite icon);

        Sprite GetIcon(object element, IListMenuManager manager, IListMenuArg arg);

        string GetStyle(object element, IListMenuManager manager, IListMenuArg arg);
    }
}
