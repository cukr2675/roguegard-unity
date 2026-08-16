using Lysionium;
using Roguegard.Device;

namespace Roguegard
{
    public class LobbyDoorBeApplied : ReferableScript, IApplyRogueMethod
    {
        private LobbyDoorBeApplied() { }

        IRogueMethodTarget ISkillDescribable.Target => null;
        IRogueMethodRange ISkillDescribable.Range => null;
        int ISkillDescribable.RequiredMp => 0;
        Spanning<IKeyword> ISkillDescribable.AmmoCategories => Spanning<IKeyword>.Empty;

        private static readonly DungeonSelectionScreen dungeonSelectionScreen = new();

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                RogueDevice.Primary.AddScreen(dungeonSelectionScreen, user, null, RogueMethodArgument.Identity);
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

        private class DungeonSelectionScreen : RogueListuiScreen
        {
            private readonly ScrollMenuViewData<ISelectOption<MMgr, MArg>, MMgr> view = new()
            {
            };

            public DungeonSelectionScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(RoguegardSettings.DungeonSelectOption, manager)
                    ?
                    .NameFrom((o, m) => o.GetName(m, Arg))
                    .OnClick((o, m) => o.Click(m, "Click", Arg))
                    .StyleFrom((o, m) => o.GetStyle(m, Arg))
                    .Build();
                };
            }
        }
    }
}
