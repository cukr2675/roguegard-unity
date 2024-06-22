using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    public class PutIntoChestCommandMenu : IListMenu
    {
        private readonly object[] selectOptions;

        public PutIntoChestCommandMenu()
        {
            selectOptions = new object[] { new PutIn(), ExitListMenuSelectOption.Instance };
        }

        void IListMenu.OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.GetView(DeviceKw.MenuCommand).OpenView(SelectOptionPresenter.Instance, selectOptions, manager, self, user, arg);
        }

        private class PutIn : BaseListMenuSelectOption
        {
            public override string Name => "すべて入れる";

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.Done();

                var chestInfo = ChestInfo.GetInfo(arg.TargetObj);
                var selfObjs = self.Space.Objs;
                for (int i = 0; i < selfObjs.Count; i++)
                {
                    var obj = selfObjs[i];
                    if (obj == null || !obj.CanStack(arg.Tool)) continue;

                    default(IActiveRogueMethodCaller).PutIn(self, arg.TargetObj, chestInfo, obj, 0f);
                }

                manager.AddObject(DeviceKw.EnqueueSE, MainInfoKw.Put);
                RogueDevice.Add(DeviceKw.AppendText, "持っている");
                RogueDevice.Add(DeviceKw.AppendText, arg.Tool);
                RogueDevice.Add(DeviceKw.AppendText, "をすべて");
                RogueDevice.Add(DeviceKw.AppendText, arg.TargetObj);
                RogueDevice.Add(DeviceKw.AppendText, "に入れた\n");
            }
        }
    }
}
