using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public class SelectObjMenu : RogueMenuScreen
    {
        private readonly IDeviceCommandAction callback;
        private readonly List<RogueObj> list = new();

        private readonly ScrollViewTemplate<RogueObj, RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
        };

        public SelectObjMenu(IDeviceCommandAction callback)
        {
            this.callback = callback;
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            list.Clear();
            for (int i = 0; i < arg.Self.Space.Objs.Count; i++)
            {
                list.Add(arg.Self.Space.Objs[i]);
            }

            view.ShowTemplate(list, manager, arg)
                ?
                .ElementNameFrom((obj, manager, arg) =>
                {
                    return obj.GetName();
                })

                .OnClickElement((obj, manager, arg) =>
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
