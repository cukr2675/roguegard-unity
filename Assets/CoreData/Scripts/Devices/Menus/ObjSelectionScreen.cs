using Lysionium;

namespace Roguegard.Device
{
    public class ObjSelectionScreen : RogueListuiScreen
    {
        private readonly ScrollMenuViewData<RogueObj, MMgr> view = new()
        {
        };

        public ObjSelectionScreen(IDeviceCommand callback)
        {
            OnOpenScreen += (manager) =>
            {
                view.Show(Arg.Self.Space.Objs, manager)
                ?
                .Filter(obj => obj != null)

                .NameFrom(obj => obj.GetName())

                .OnClick((obj, manager) =>
                {
                    var device = RogueDeviceEffect.Get(Arg.Self);
                    var callbackArg = new RogueMethodArgument(tool: obj);
                    device.SetDeviceCommand(callback, Arg.Self, callbackArg);
                    manager.Done();
                })

                .Build();
            };
        }
    }
}
