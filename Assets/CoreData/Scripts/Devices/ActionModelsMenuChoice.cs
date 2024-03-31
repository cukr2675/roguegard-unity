using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class ActionModelsMenuChoice : BaseModelsMenuChoice
    {
        private readonly string choiceName;
        private readonly ModelsMenuAction action;

        public override string Name => choiceName;

        public ActionModelsMenuChoice(string choiceName, ModelsMenuAction action)
        {
            this.choiceName = choiceName;
            this.action = action;
        }

        public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            action.Invoke(root, self, user, arg);
        }
    }
}
