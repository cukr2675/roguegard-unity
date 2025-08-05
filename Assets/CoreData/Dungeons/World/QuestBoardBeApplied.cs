using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using System.Collections.Generic;

namespace Roguegard
{
    public class QuestBoardBeApplied : ReferableScript, IApplyRogueMethod
    {
        private QuestBoardBeApplied() { }

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

        public class RogueMenu : RogueMenuScreen
        {
            private static readonly List<DungeonQuest> elms = new();
            private static readonly QuestViewMenu nextMenu = new();

            private readonly ScrollMenuViewData<DungeonQuest, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                elms.Clear();
                for (int i = 0; i < 4; i++)
                {
                    var quest = RoguegardSettings.DungeonQuestGenerator.GenerateQuest(RogueRandom.Primary);
                    elms.Add(quest);
                }

                view.Show(elms, manager, arg)
                    ?
                    .NameFrom((quest, manager, arg) =>
                    {
                        return quest.Caption;
                    })

                    .OnClick((quest, manager, arg) =>
                    {
                        manager.PushMenuScreen(nextMenu, arg.Self, other: quest);
                    })

                    .Build();
            }

            private class QuestViewMenu : RogueMenuScreen
            {
                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    var quest = (DungeonQuest)arg.Arg.Other;

                    var summary = RoguegardSubviews.GetSummary(manager);
                    summary.SetQuest(arg.Self, quest, true, manager);
                    summary.Show();
                }
            }
        }
    }
}
