using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    public class LobbyDoorBeApplied : ReferableScript, IApplyRogueMethod
    {
        private LobbyDoorBeApplied() { }

        IRogueMethodTarget ISkillDescription.Target => null;
        IRogueMethodRange ISkillDescription.Range => null;
        int ISkillDescription.RequiredMP => 0;
        Spanning<IKeyword> ISkillDescription.AmmoCategories => Spanning<IKeyword>.Empty;

        private static readonly RogueMenu rogueMenu = new RogueMenu();

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                RogueDevice.Primary.AddMenu(rogueMenu, user, null, RogueMethodArgument.Identity);
                return true;
            }
            else
            {
                return false;
            }
        }

        int ISkillDescription.GetATK(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }

        private class RogueMenu : IListMenu
        {
            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var dungeonSelectOptions = RoguegardSettings.DungeonSelectOption;
                var scroll = manager.GetView(DeviceKw.MenuScroll);
                scroll.OpenView(SelectOptionPresenter.Instance, dungeonSelectOptions, manager, self, user, arg);
                ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
            }
        }
    }
}
