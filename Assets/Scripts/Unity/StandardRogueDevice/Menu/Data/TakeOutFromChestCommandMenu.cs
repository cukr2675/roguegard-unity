using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    public class TakeOutFromChestCommandMenu : IModelsMenu
    {
        private readonly object[] choices;

        public TakeOutFromChestCommandMenu()
        {
            choices = new object[] { new TakeOut(), ExitModelsMenuChoice.Instance };
        }

        void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.Get(DeviceKw.MenuCommand).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, self, user, arg);
        }

        private class TakeOut : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "取り出す";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Done();

                var chestInfo = ChestInfo.GetInfo(arg.TargetObj);
                default(IActiveRogueMethodCaller).TakeOut(self, arg.TargetObj, chestInfo, arg.Tool, 0f);

                root.AddObject(DeviceKw.EnqueueSE, MainInfoKw.PickUp);
                RogueDevice.Add(DeviceKw.AppendText, arg.TargetObj);
                RogueDevice.Add(DeviceKw.AppendText, "から");
                RogueDevice.Add(DeviceKw.AppendText, arg.Tool);
                RogueDevice.Add(DeviceKw.AppendText, "を取り出した\n");
            }
        }
    }
}
