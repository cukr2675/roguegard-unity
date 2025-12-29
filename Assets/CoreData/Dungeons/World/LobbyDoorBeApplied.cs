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
            private readonly ScrollMenuViewData<ISelectOption<MMgr, MArg>, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                view.Show(RoguegardSettings.DungeonSelectOption, manager, arg)
                    ?
                    .Build();
            }
        }
    }
}
