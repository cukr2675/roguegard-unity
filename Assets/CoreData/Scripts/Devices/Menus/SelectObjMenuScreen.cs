using Lysionium;

namespace Roguegard.Device
{
    public class SelectObjMenuScreen : RogueMenuScreen
    {
        private readonly ScrollMenuViewData<RogueObj, MMgr, MArg> view = new()
        {
        };

        private readonly IDeviceCommand callback;

        public SelectObjMenuScreen(IDeviceCommand callback)
        {
            this.callback = callback;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
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
