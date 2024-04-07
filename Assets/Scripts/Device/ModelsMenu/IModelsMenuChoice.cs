using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// <see cref="ChoiceListPresenter"/> のモデルとして扱うインターフェース。
    /// </summary>
    public interface IModelsMenuChoice
    {
        string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);
    }
}
