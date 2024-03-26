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

        private object[] models;

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (models == null)
            {
                models = new object[]
                {
                    new Text() { parent = this },
                    new CancelButton() { parent = this }
                };
            }

            Interrupt = false;
            var loading = root.Get(DeviceKw.MenuLoading);
            loading.OpenView(ChoicesModelsMenuItemController.Instance, models, root, null, null, RogueMethodArgument.Identity);
        }

        private class Text : IModelsMenuChoice
        {
            public SynchronizeMenu parent;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "ê¢äEÇ∆ìØä˙íÜÅc";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var loading = root.Get(DeviceKw.MenuLoading);
                loading.SetPosition(parent.Progress);
            }
        }

        private class CancelButton : IModelsMenuChoice
        {
            public SynchronizeMenu parent;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "ìØä˙ÇíÜé~";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                parent.Interrupt = true;
            }
        }
    }
}
