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
        private readonly MainMenuViewTemplate<MMgr, MArg> view = new()
        {
            PrimaryCommandSubViewName = StandardSubViewTable.SecondaryCommandName,
        };

        public override bool IsIncremental => true;

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            view.ShowTemplate(manager, arg)
                ?
                .Option("取り出す", (manager, arg) =>
                {
                    manager.Done();

                    var chestInfo = ChestInfo.GetInfo(arg.Arg.TargetObj);
                    default(IActiveRogueMethodCaller).TakeOut(arg.Self, arg.Arg.TargetObj, chestInfo, arg.Arg.Tool, 0f);

                    manager.AddObject(DeviceKw.EnqueueSE, MainInfoKw.PickUp);
                    RogueDevice.Add(DeviceKw.AppendText, arg.Arg.TargetObj);
                    RogueDevice.Add(DeviceKw.AppendText, "から");
                    RogueDevice.Add(DeviceKw.AppendText, arg.Arg.Tool);
                    RogueDevice.Add(DeviceKw.AppendText, "を取り出した\n");
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
