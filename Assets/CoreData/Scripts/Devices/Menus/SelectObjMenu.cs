using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class SelectObjMenu : IListMenu
    {
        private readonly Presenter presenter;

        public SelectObjMenu(IDeviceCommandAction callback)
        {
            presenter = new Presenter() { callback = callback };
        }

        public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.GetView(DeviceKw.MenuScroll).OpenView(presenter, self.Space.Objs, manager, self, user, arg);
        }

        private class Presenter : IElementPresenter
        {
            public IDeviceCommandAction callback;

            public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var obj = (RogueObj)element;
                return obj.GetName();
            }

            public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var obj = (RogueObj)element;
                var device = RogueDeviceEffect.Get(self);
                var callbackArg = new RogueMethodArgument(tool: obj);
                device.SetDeviceCommand(callback, self, callbackArg);
                manager.Done();
            }
        }
    }
}
