using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// <see cref="ChoicesModelsMenuItemController"/> のモデルとして扱うインターフェース。
    /// </summary>
    public interface IModelsMenuChoice
    {
        string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);
    }
}
