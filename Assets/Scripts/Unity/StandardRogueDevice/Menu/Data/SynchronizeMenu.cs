using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    internal class SynchronizeMenu : IModelsMenu
    {
        public bool Interrupt { get; private set; }
        public float Progress { get; set; }

        private LoadingModelsMenu loadingMenu;

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (loadingMenu == null) { loadingMenu = new LoadingModelsMenu("¢ŠE‚Æ“¯Šú’†c", "“¯Šú‚ğ’†~", Stop, Update); }

            Interrupt = false;
            loadingMenu.OpenMenu(root, null, null, RogueMethodArgument.Identity);
        }

        private void Stop(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            Interrupt = true;
        }

        private void Update(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var loading = root.Get(DeviceKw.MenuLoading);
            loading.SetPosition(Progress);
        }
    }
}
