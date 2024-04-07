using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class ExitModelsMenuChoice : BaseModelsMenuChoice
    {
        public static ExitModelsMenuChoice Instance { get; } = new ExitModelsMenuChoice(null);

        private static readonly object[] single = new[] { Instance };

        public override string Name => ":Exit";

        private readonly ModelsMenuAction action;

        public ExitModelsMenuChoice(ModelsMenuAction action)
        {
            this.action = action;
        }

        public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            action?.Invoke(root, self, user, arg);
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
            root.Back();
        }

        public static void OpenLeftAnchorExit(IModelsMenuRoot root)
        {
            var leftAnchor = root.Get(DeviceKw.MenuLeftAnchor);
            leftAnchor.OpenView(ChoiceListPresenter.Instance, single, root, null, null, RogueMethodArgument.Identity);
        }
    }
}
