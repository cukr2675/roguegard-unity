using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class LoadingListMenu : IListMenu
    {
        private readonly object[] elms;

        public LoadingListMenu(string text, string buttonText, ListMenuAction buttonAction, ListMenuAction updateAction = null)
        {
            elms = new object[]
            {
                new ActionListMenuSelectOption(text, updateAction ?? Wait),
                new ActionListMenuSelectOption(buttonText, buttonAction)
            };
        }

        public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.GetView(DeviceKw.MenuLoading).OpenView(SelectOptionPresenter.Instance, elms, manager, self, user, arg);
        }

        private static void Wait(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
        }
    }
}
