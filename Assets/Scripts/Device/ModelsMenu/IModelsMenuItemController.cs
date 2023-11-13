using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// メニューのモデルに対する操作を表すインターフェース。
    /// 一つの <see cref="IModelsMenu"/> が複数の <see cref="IModelsMenuItemController"/> を持つ可能性がある。
    /// </summary>
    public interface IModelsMenuItemController
    {
        string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);
    }
}
