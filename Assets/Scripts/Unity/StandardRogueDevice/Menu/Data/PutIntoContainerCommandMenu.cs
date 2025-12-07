using Lysionium;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    public class PutIntoContainerCommandMenu : RogueMenuScreen
    {
        private readonly MainMenuViewData<MMgr, MArg> view = new()
        {
            PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
        };

        public override bool IsIncremental => true;

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            view.Show(manager, arg)
                ?
                .Option("すべて入れる", (manager, arg) =>
                {
                    manager.Done();

                    var containerInfo = ContainerInfo.GetInfo(arg.Arg.TargetObj);
                    var selfObjs = arg.Self.Space.Objs;
                    for (int i = 0; i < selfObjs.Length; i++) // アイテムの移動でオブジェクト数が増加する可能性がある
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
            view.Hide(manager, back);
        }
    }
}
