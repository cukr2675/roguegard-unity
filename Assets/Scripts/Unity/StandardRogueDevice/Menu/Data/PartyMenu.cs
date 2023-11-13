using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMenu : IModelsMenu
    {
        private readonly ItemController itemController;

        public PartyMenu(CaptionWindow captionWindow, PartyMemberMenu memberMenu)
        {
            itemController = new ItemController() { menu = memberMenu };
        }

        void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var partyMembers = self.Main.Stats.Party.Members;
            var scroll = (ScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
            scroll.OpenView(itemController, partyMembers, root, self, user, arg);
            scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
        }

        private class ItemController : IPartyMenuItemController
        {
            public PartyMemberMenu menu;

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var partyMember = (RogueObj)model;
                return partyMember.GetName();
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var partyMember = (RogueObj)model;
                var openArg = new RogueMethodArgument(targetObj: partyMember);
                var backArg = RogueMethodArgument.Identity;
                root.OpenMenuAsDialog(menu, self, user, openArg, backArg);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }
    }
}
