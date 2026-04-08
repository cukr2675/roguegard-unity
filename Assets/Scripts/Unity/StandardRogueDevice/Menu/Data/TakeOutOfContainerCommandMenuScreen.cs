using Lysionium;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    public class TakeOutOfContainerCommandMenuScreen : RogueListuiScreen
    {
        private readonly MainMenuViewData<MMgr> view = new()
        {
            PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
        };

        public TakeOutOfContainerCommandMenuScreen()
        {
            OnOpenScreen += (manager) =>
            {
                view.Show(manager)
                ?
                .Option("取り出す", (manager) =>
                {
                    manager.Done();

                    var containerInfo = ContainerInfo.GetInfo(Arg.Arg.TargetObj);
                    default(IActiveRogueMethodCaller).TakeOut(Arg.Self, Arg.Arg.TargetObj, containerInfo, Arg.Arg.Tool, 0f);

                    RogueDevice.Add(DeviceKw.AppendText, Arg.Arg.TargetObj);
                    RogueDevice.Add(DeviceKw.AppendText, "から");
                    RogueDevice.Add(DeviceKw.AppendText, Arg.Arg.Tool);
                    RogueDevice.Add(DeviceKw.AppendText, "を取り出した\n");
                }, "PickUp")

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
