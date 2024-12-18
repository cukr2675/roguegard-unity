using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// メニューの画面単位のインターフェース。
    /// 一つの画面につき複数の <see cref="IElementPresenter"/> を持つ可能性がある。
    /// </summary>
    [System.Obsolete]
    public interface IListMenu
    {
        void OpenMenu(object manager, RogueObj self, RogueObj user, in RogueMethodArgument arg);
    }
}
