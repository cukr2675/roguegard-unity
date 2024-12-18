using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// リスト処理ビュー。
    /// 設定された <see cref="IElementPresenter"/> とリストをもとに UI を表示するインターフェース。
    /// </summary>
    [System.Obsolete]
    public interface IElementsView
    {
        void OpenView<T>(
            IElementPresenter presenter, Spanning<T> list, object manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        float GetPosition();

        void SetPosition(float position);
    }
}
