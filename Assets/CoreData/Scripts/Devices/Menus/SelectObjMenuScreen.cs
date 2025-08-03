using Lysionium;
using System.Collections.Generic;

namespace Roguegard.Device
{
    public class SelectObjMenuScreen : RogueMenuScreen
    {
        private readonly IDeviceCommand callback;
        private readonly List<RogueObj> list = new();

        private readonly ScrollViewData<RogueObj, MMgr, MArg> view = new()
        {
        };

        public SelectObjMenuScreen(IDeviceCommand callback)
        {
            this.callback = callback;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            list.Clear();
            foreach (var obj in arg.Self.Space.Objs)
            {
                list.Add(obj);
            }

            view.Show(list, manager, arg)
                ?
                .NameFrom((obj, manager, arg) =>
                {
                    return obj.GetName();
                })

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
