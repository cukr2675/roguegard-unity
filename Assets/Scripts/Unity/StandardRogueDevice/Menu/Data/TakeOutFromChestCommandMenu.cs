using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    public class TakeOutFromChestCommandMenu : IListMenu
    {
        private readonly object[] selectOptions;

        public TakeOutFromChestCommandMenu()
        {
            selectOptions = new object[] { new TakeOut(), ExitListMenuSelectOption.Instance };
        }

        void IListMenu.OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.GetView(DeviceKw.MenuCommand).OpenView(SelectOptionPresenter.Instance, selectOptions, manager, self, user, arg);
        }

        private class TakeOut : BaseListMenuSelectOption
        {
            public override string Name => "取り出す";

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.Done();

                var chestInfo = ChestInfo.GetInfo(arg.TargetObj);
                default(IActiveRogueMethodCaller).TakeOut(self, arg.TargetObj, chestInfo, arg.Tool, 0f);

                manager.AddObject(DeviceKw.EnqueueSE, MainInfoKw.PickUp);
                RogueDevice.Add(DeviceKw.AppendText, arg.TargetObj);
                RogueDevice.Add(DeviceKw.AppendText, "から");
                RogueDevice.Add(DeviceKw.AppendText, arg.Tool);
                RogueDevice.Add(DeviceKw.AppendText, "を取り出した\n");
            }
        }
    }
}
