using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class ExitModelsMenuChoice : IModelsMenuChoice
    {
        public static ExitModelsMenuChoice Instance { get; } = new ExitModelsMenuChoice(null);

        private static readonly object[] single = new[] { Instance };

        private ModelsMenuAction Action { get; }

        public ExitModelsMenuChoice(ModelsMenuAction action)
        {
            Action = action;
        }

        public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            return ":Exit";
        }

        public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            Action?.Invoke(root, self, user, arg);
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
            root.Back();
        }

        public static void OpenLeftAnchorExit(IModelsMenuRoot root)
        {
            var leftAnchor = root.Get(DeviceKw.MenuLeftAnchor);
            leftAnchor.OpenView(ChoicesModelsMenuItemController.Instance, single, root, null, null, RogueMethodArgument.Identity);
        }
    }
}
