using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class SelectObjMenu : IModelsMenu
    {
        private readonly Presenter presenter;

        public SelectObjMenu(IDeviceCommandAction callback)
        {
            presenter = new Presenter() { callback = callback };
        }

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.Get(DeviceKw.MenuScroll).OpenView(presenter, self.Space.Objs, root, self, user, arg);
        }

        private class Presenter : IModelListPresenter
        {
            public IDeviceCommandAction callback;

            public string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var obj = (RogueObj)model;
                return obj.GetName();
            }

            public void ActivateItem(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
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
