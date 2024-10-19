using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
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

        private class RogueMenu : RogueMenuScreen
        {
            private readonly ScrollViewTemplate<IListMenuSelectOption, RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            private readonly List<IListMenuSelectOption> selectOptions = new();

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                var dungeonSelectOptions = RoguegardSettings.DungeonSelectOption;
                selectOptions.Clear();
                for (int i = 0; i < dungeonSelectOptions.Count; i++)
                {
                    selectOptions.Add(dungeonSelectOptions[i]);
                }

                view.Show(selectOptions, manager, arg)
                    ?.ElementNameGetter(SelectOptionHandler.Instance.GetName)
                    .OnClickElement(SelectOptionHandler.Instance.HandleClick)
                    .Build();
            }
        }
    }
}
