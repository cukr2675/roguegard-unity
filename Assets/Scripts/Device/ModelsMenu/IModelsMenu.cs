using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// メニューの画面単位のインターフェース。
    /// 一つの画面につき複数の <see cref="IModelListPresenter"/> を持つ可能性がある。
    /// </summary>
    public interface IModelsMenu
    {
        void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);
    }
}
