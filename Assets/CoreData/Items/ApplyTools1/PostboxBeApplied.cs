using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class PostboxBeApplied : BaseApplyRogueMethod
    {
        private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                menu ??= new Menu();
                RogueDevice.Primary.AddMenu(menu, user, null, new(tool: self));
                return false;
            }
            else
            {
                return false;
            }
        }

        private class Menu : BaseScrollListMenu<RoguePost>
        {
            private static readonly DetailsMenu nextMenu = new();

            protected override Spanning<RoguePost> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var info = PostboxInfo.Get(arg.Tool);
                return info.Posts;
            }

            protected override string GetItemName(RoguePost element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return element.Name;
            }

            protected override void ActivateItem(RoguePost element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.OpenMenu(nextMenu, null, null, new(other: element));
            }
        }

        private class DetailsMenu : IListMenu
        {
            private List<object> elms = new();

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var post = arg.Other;

                elms.Clear();
                elms.Add(post);
                manager.GetView(DeviceKw.MenuDetails).OpenView(SelectOptionPresenter.Instance, elms, manager, null, null, RogueMethodArgument.Identity);
                ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
            }
        }
    }
}
