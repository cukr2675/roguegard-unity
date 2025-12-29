using Lysionium;

namespace Roguegard.Device
{
    public class ObjSelectionScreen : RogueListuiScreen
    {
        private readonly ScrollMenuViewData<RogueObj, MMgr, MArg> view = new()
        {
        };

        private readonly IDeviceCommand callback;

        public ObjSelectionScreen(IDeviceCommand callback)
        {
            this.callback = callback;
        }

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            view.Show(arg.Self.Space.Objs, manager, arg)
                ?
                .Filter(obj => obj != null)

                .NameFrom(obj => obj.GetName())

                .OnClick((obj, manager, arg) =>
                {
                    var device = RogueDeviceEffect.Get(arg.Self);
                    var callbackArg = new RogueMethodArgument(tool: obj);
                    device.SetDeviceCommand(callback, arg.Self, callbackArg);
                    manager.Done();
                })

                .Build();
        }
    }
}
