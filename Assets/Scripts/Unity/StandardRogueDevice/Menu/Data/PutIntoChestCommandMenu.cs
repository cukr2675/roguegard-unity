using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    public class PutIntoChestCommandMenu : IModelsMenu
    {
        private readonly object[] choices;

        public PutIntoChestCommandMenu()
        {
            choices = new object[] { new PutIn(), ExitModelsMenuChoice.Instance };
        }

        void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.Get(DeviceKw.MenuCommand).OpenView(ChoiceListPresenter.Instance, choices, root, self, user, arg);
        }

        private class PutIn : BaseModelsMenuChoice
        {
            public override string Name => "すべて入れる";

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Done();

                var chestInfo = ChestInfo.GetInfo(arg.TargetObj);
                var selfObjs = self.Space.Objs;
                for (int i = 0; i < selfObjs.Count; i++)
                {
                    var obj = selfObjs[i];
                    if (obj == null || !obj.CanStack(arg.Tool)) continue;

                    default(IActiveRogueMethodCaller).PutIn(self, arg.TargetObj, chestInfo, obj, 0f);
                }

                root.AddObject(DeviceKw.EnqueueSE, MainInfoKw.Put);
                RogueDevice.Add(DeviceKw.AppendText, "持っている");
                RogueDevice.Add(DeviceKw.AppendText, arg.Tool);
                RogueDevice.Add(DeviceKw.AppendText, "をすべて");
                RogueDevice.Add(DeviceKw.AppendText, arg.TargetObj);
                RogueDevice.Add(DeviceKw.AppendText, "に入れた\n");
            }
        }
    }
}
