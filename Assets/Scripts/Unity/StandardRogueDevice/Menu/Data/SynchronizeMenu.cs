using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    internal class SynchronizeMenu : IListMenu
    {
        public bool Interrupt { get; private set; }
        public float Progress { get; set; }

        private LoadingListMenu loadingMenu;

        public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (loadingMenu == null) { loadingMenu = new LoadingListMenu("¢ŠE‚Æ“¯Šú’†c", "“¯Šú‚ğ’†~", Stop, Update); }

            Interrupt = false;
            loadingMenu.OpenMenu(manager, null, null, RogueMethodArgument.Identity);
        }

        private void Stop(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            Interrupt = true;
        }

        private void Update(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var loading = manager.GetView(DeviceKw.MenuLoading);
            loading.SetPosition(Progress);
        }
    }
}
