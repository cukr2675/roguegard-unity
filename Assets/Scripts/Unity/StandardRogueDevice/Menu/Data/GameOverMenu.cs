using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    public class GameOverMenu : IModelsMenu
    {
        private static readonly object[] choices = new[] { new Next() };

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.Get(DeviceKw.MenuLog).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, self, user, arg);
        }

        private class Next : IModelsMenuChoice
        {
            private static readonly NextMenu nextMenu = new NextMenu();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return null;
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(nextMenu, self, user, arg, arg);
            }
        }

        private class NextMenu : IModelsMenu
        {
            public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
            {
                var summary = (IResultMenuView)root.Get(DeviceKw.MenuSummary);
                summary.OpenView(ChoicesModelsMenuItemController.Instance, Spanning<object>.Empty, root, player, user, arg);
                summary.SetGameOver(player, arg.TargetObj);
            }
        }
    }
}
