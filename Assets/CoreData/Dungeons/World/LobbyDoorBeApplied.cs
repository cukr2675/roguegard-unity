using Lysionium;
using Roguegard.Device;
using System.Collections.Generic;

namespace Roguegard
{
    public class LobbyDoorBeApplied : ReferableScript, IApplyRogueMethod
    {
        private LobbyDoorBeApplied() { }

        IRogueMethodTarget ISkillDescribable.Target => null;
        IRogueMethodRange ISkillDescribable.Range => null;
        int ISkillDescribable.RequiredMp => 0;
        Spanning<IKeyword> ISkillDescribable.AmmoCategories => Spanning<IKeyword>.Empty;

        private static readonly RogueMenu rogueMenu = new();

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

        int ISkillDescribable.GetAtk(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }

        private class RogueMenu : RogueMenuScreen
        {
            private readonly ScrollMenuViewData<ISelectOption, MMgr, MArg> view = new()
            {
            };

            private readonly List<ISelectOption> selectOptions = new();

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                selectOptions.Clear();
                foreach (var option in RoguegardSettings.DungeonSelectOption)
                {
                    selectOptions.Add(option);
                }

                view.Show(selectOptions, manager, arg)
                    ?
                    .Build();
            }
        }
    }
}
