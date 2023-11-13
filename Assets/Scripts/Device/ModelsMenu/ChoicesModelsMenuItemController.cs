using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// モデルのリストではなく選択肢を扱いたいときに使用する <see cref="IModelsMenuItemController"/> 。
    /// </summary>
    public class ChoicesModelsMenuItemController : IModelsMenuItemController
    {
        public static ChoicesModelsMenuItemController Instance { get; } = new ChoicesModelsMenuItemController();

        public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var command = (IModelsMenuChoice)model;
            return command.GetName(root, self, user, arg);
        }

        public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var command = (IModelsMenuChoice)model;
            command.Activate(root, self, user, arg);
        }
    }
}
