using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    public class TakeOutFromChestCommandMenu : RogueMenuScreen
    {
        private readonly IListMenuSelectOption[] selectOptions;

        private readonly CommandListViewTemplate<IListMenuSelectOption, RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
        };

        public TakeOutFromChestCommandMenu()
        {
            selectOptions = new IListMenuSelectOption[]
            {
                ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("取り出す", (manager, arg) =>
                {
                    manager.Done();
                    
                    var chestInfo = ChestInfo.GetInfo(arg.Arg.TargetObj);
                    default(IActiveRogueMethodCaller).TakeOut(arg.Self, arg.Arg.TargetObj, chestInfo, arg.Arg.Tool, 0f);
                    
                    manager.AddObject(DeviceKw.EnqueueSE, MainInfoKw.PickUp);
                    RogueDevice.Add(DeviceKw.AppendText, arg.Arg.TargetObj);
                    RogueDevice.Add(DeviceKw.AppendText, "から");
                    RogueDevice.Add(DeviceKw.AppendText, arg.Arg.Tool);
                    RogueDevice.Add(DeviceKw.AppendText, "を取り出した\n");
                }),
                ExitListMenuSelectOption.Instance
            };
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            view.ShowTemplate(selectOptions, manager, arg)
                ?.Build();
        }
    }
}
