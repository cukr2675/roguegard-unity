using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    public class NotebookBeApplied : BaseApplyRogueMethod
    {
        private static readonly Menu menu = new Menu();

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            RogueDevice.Primary.AddMenu(menu, user, null, new(tool: self));
            return false;
        }

        private class Menu : IModelsMenu
        {
            private static readonly IModelsMenuChoice exit = new ActionModelsMenuChoice("•Â‚¶‚é", Exit);

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var memo = arg.Tool;
                var text = NotebookInfo.GetText(memo);
                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuTextEditor);
                scroll.OpenView(ChoicesModelsMenuItemController.Instance, Spanning<object>.Empty, root, self, null, new(other: text));
                scroll.ShowExitButton(exit);
            }

            private static void Exit(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                NotebookInfo.SetTo(arg.Tool, (string)arg.Other);
                root.Done();
            }
        }
    }
}
