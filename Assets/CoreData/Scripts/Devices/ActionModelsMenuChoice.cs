using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class ActionModelsMenuChoice : IModelsMenuChoice
    {
        private readonly string choiceName;
        private readonly ModelsMenuAction action;

        public ActionModelsMenuChoice(string choiceName, ModelsMenuAction action)
        {
            this.choiceName = choiceName;
            this.action = action;
        }

        public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            return choiceName;
        }

        public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            action.Invoke(root, self, user, arg);
        }
    }
}
