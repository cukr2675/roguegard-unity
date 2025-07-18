using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lysionium;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    public class PutIntoContainerCommandMenu : RogueMenuScreen
    {
        private readonly MainMenuViewTemplate<MMgr, MArg> view = new()
        {
            PrimaryCommandSubviewName = StandardSubviewTable.SecondaryCommandName,
        };

        public override bool IsIncremental => true;

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            view.ShowTemplate(manager, arg)
                ?
                .Option("すべて入れる", (manager, arg) =>
                {
                    manager.Done();

                    var containerInfo = ContainerInfo.GetInfo(arg.Arg.TargetObj);
                    var selfObjs = arg.Self.Space.Objs;
                    for (int i = 0; i < selfObjs.Count; i++) // アイテムの移動でオブジェクト数が増加する可能性がある
                    {
                        var obj = selfObjs[i];
                        if (obj == null || !obj.CanStack(arg.Arg.Tool)) continue;

                        default(IActiveRogueMethodCaller).PutIn(arg.Self, arg.Arg.TargetObj, containerInfo, obj, 0f);
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

        public override void CloseScreenView(MMgr manager, bool back)
        {
            view.HideTemplate(manager, back);
        }
    }
}
