using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    public class PutIntoChestCommandMenu : RogueMenuScreen
    {
        private readonly ISelectOption[] selectOptions;

        private readonly CommandListViewTemplate<ISelectOption, RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
        };

        public PutIntoChestCommandMenu()
        {
            selectOptions = new ISelectOption[]
            {
                SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("すべて入れる", (manager, arg) =>
                {
                    manager.Done();
                    
                    var chestInfo = ChestInfo.GetInfo(arg.Arg.TargetObj);
                    var selfObjs = arg.Self.Space.Objs;
                    for (int i = 0; i < selfObjs.Count; i++)
                    {
                        var obj = selfObjs[i];
                        if (obj == null || !obj.CanStack(arg.Arg.Tool)) continue;
                        
                        default(IActiveRogueMethodCaller).PutIn(arg.Self, arg.Arg.TargetObj, chestInfo, obj, 0f);
                    }
                    
                    manager.AddObject(DeviceKw.EnqueueSE, MainInfoKw.Put);
                    RogueDevice.Add(DeviceKw.AppendText, "持っている");
                    RogueDevice.Add(DeviceKw.AppendText, arg.Arg.Tool);
                    RogueDevice.Add(DeviceKw.AppendText, "をすべて");
                    RogueDevice.Add(DeviceKw.AppendText, arg.Arg.TargetObj);
                    RogueDevice.Add(DeviceKw.AppendText, "に入れた\n");
                }),
                BackSelectOption.Instance
            };
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            view.ShowTemplate(selectOptions, manager, arg)
                ?.Build();
        }
    }
}
