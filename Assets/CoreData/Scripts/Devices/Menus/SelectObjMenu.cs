using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class SelectObjMenu : IModelsMenu
    {
        private readonly ItemController controller;

        public SelectObjMenu(IDeviceCommandAction callback)
        {
            controller = new ItemController() { callback = callback };
        }

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.Get(DeviceKw.MenuScroll).OpenView(controller, self.Space.Objs, root, self, user, arg);
        }

        private class ItemController : IModelsMenuItemController
        {
            public IDeviceCommandAction callback;

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var obj = (RogueObj)model;
                return obj.GetName();
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var obj = (RogueObj)model;
                var device = RogueDeviceEffect.Get(self);
                var callbackArg = new RogueMethodArgument(tool: obj);
                device.SetDeviceCommand(callback, self, callbackArg);
                root.Done();
            }
        }
    }
}
