using Lysionium;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    public class PutIntoContainerCommandMenuScreen : RogueListuiScreen
    {
        private readonly MainMenuViewData<MMgr> view = new()
        {
            PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
        };

        public PutIntoContainerCommandMenuScreen()
        {
            OnOpenScreen += (manager) =>
            {
                view.Show(manager)
                ?
                .Option("すべて入れる", (manager) =>
                {
                    manager.Done();

                    var containerInfo = ContainerInfo.GetInfo(Arg.Arg.TargetObj);
                    var selfObjs = Arg.Self.Space.Objs;
                    for (int i = 0; i < selfObjs.Length; i++) // アイテムの移動でオブジェクト数が増加する可能性がある
                    {
                        var obj = selfObjs[i];
                        if (obj == null || !obj.CanStack(Arg.Arg.Tool)) continue;

                        default(IActiveRogueMethodCaller).PutIn(Arg.Self, Arg.Arg.TargetObj, containerInfo, obj, 0f);
                    }

                    manager.AddObject(DeviceKw.EnqueueSE, MainInfoKw.Put);
                    RogueDevice.Add(DeviceKw.AppendText, "持っている");
                    RogueDevice.Add(DeviceKw.AppendText, Arg.Arg.Tool);
                    RogueDevice.Add(DeviceKw.AppendText, "をすべて");
                    RogueDevice.Add(DeviceKw.AppendText, Arg.Arg.TargetObj);
                    RogueDevice.Add(DeviceKw.AppendText, "に入れた\n");
                })

                .Back()

                .Build();
            };

            OnCloseScreenView += (manager, back) =>
            {
                view.Hide(manager, back);
            };
        }
    }
}
