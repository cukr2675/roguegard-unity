using Lysionium;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    public class TakeOutOfContainerCommandMenu : RogueMenuScreen
    {
        private readonly MainMenuViewData<MMgr, MArg> view = new()
        {
            PrimaryCommandSubviewName = StandardSubviewTable.SecondaryCommandName,
        };

        public override bool IsIncremental => true;

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            view.Show(manager, arg)
                ?
                .Option("取り出す", (manager, arg) =>
                {
                    manager.Done();

                    var containerInfo = ContainerInfo.GetInfo(arg.Arg.TargetObj);
                    default(IActiveRogueMethodCaller).TakeOut(arg.Self, arg.Arg.TargetObj, containerInfo, arg.Arg.Tool, 0f);

                    RogueDevice.Add(DeviceKw.AppendText, arg.Arg.TargetObj);
                    RogueDevice.Add(DeviceKw.AppendText, "から");
                    RogueDevice.Add(DeviceKw.AppendText, arg.Arg.Tool);
                    RogueDevice.Add(DeviceKw.AppendText, "を取り出した\n");
                }, "PickUp")

                .Back()

                .Build();
        }

        public override void CloseScreenView(MMgr manager, bool back)
        {
            view.Hide(manager, back);
        }
    }
}
