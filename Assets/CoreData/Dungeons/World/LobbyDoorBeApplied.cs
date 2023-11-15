using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    public class LobbyDownStairsBeApplied : ReferableScript, IApplyRogueMethod
    {
        private LobbyDownStairsBeApplied() { }

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

        private class RogueMenu : IModelsMenu
        {
            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var dungeonChoices = RoguegardSettings.DungeonChoices;
                root.Get(DeviceKw.MenuScroll).OpenView(ChoicesModelsMenuItemController.Instance, dungeonChoices, root, self, user, arg);
            }
        }
    }
}
