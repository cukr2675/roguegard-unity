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
        private readonly MainMenuViewTemplate<MMgr, MArg> view = new()
        {
            PrimaryCommandSubViewName = StandardSubViewTable.SecondaryCommandName,
        };

        public override bool IsIncremental => true;

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            view.ShowTemplate(manager, arg)
                ?
                .Option("すべて入れる", (manager, arg) =>
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
                })

                .Back()

                .Build();
        }

        public override void CloseScreen(MMgr manager, bool back)
        {
            view.HideTemplate(manager, back);
        }
    }
}
