using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// モデルのリストではなく選択肢を扱いたいときに使用する <see cref="IModelListPresenter"/> 。
    /// </summary>
    public class ChoiceListPresenter : IModelListPresenter
    {
        public static ChoiceListPresenter Instance { get; } = new ChoiceListPresenter();

        public string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var command = (IModelsMenuChoice)model;
            return command.GetName(root, self, user, arg);
        }

        public void ActivateItem(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var command = (IModelsMenuChoice)model;
            command.Activate(root, self, user, arg);
        }
    }
}
