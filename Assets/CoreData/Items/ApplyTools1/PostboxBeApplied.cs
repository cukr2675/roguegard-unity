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

        private class Menu : BaseScrollModelsMenu<RoguePost>
        {
            private static readonly DetailsMenu nextMenu = new();

            protected override Spanning<RoguePost> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var info = PostboxInfo.Get(arg.Tool);
                return info.Posts;
            }

            protected override string GetItemName(RoguePost model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return model.Name;
            }

            protected override void ActivateItem(RoguePost model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.OpenMenu(nextMenu, null, null, new(other: model));
            }
        }

        private class DetailsMenu : IModelsMenu
        {
            private List<object> models = new();

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var post = arg.Other;

                models.Clear();
                models.Add(post);
                root.Get(DeviceKw.MenuDetails).OpenView(ChoiceListPresenter.Instance, models, root, null, null, RogueMethodArgument.Identity);
                ExitModelsMenuChoice.OpenLeftAnchorExit(root);
            }
        }
    }
}
