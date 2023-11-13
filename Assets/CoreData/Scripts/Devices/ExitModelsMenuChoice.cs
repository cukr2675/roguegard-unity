using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class ExitModelsMenuChoice : IModelsMenuChoice
    {
        public static ExitModelsMenuChoice Instance { get; } = new ExitModelsMenuChoice(null);

        private ModelsMenuAction Action { get; }

        public ExitModelsMenuChoice(ModelsMenuAction action)
        {
            Action = action;
        }

        public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            return "<";
        }

        public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            Action?.Invoke(root, self, user, arg);
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
            root.Back();
        }
    }
}
