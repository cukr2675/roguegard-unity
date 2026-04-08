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

        private static readonly QuestBoardScreen questBoardScreen = new();

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                RogueDevice.Primary.AddScreen(questBoardScreen, user, null, RogueMethodArgument.Identity);
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

        public class QuestBoardScreen : RogueListuiScreen
        {
            private static readonly List<DungeonQuest> questList = new();

            private readonly ScrollMenuViewData<DungeonQuest, MMgr> view = new()
            {
            };

            public QuestBoardScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    questList.Clear();
                    for (int i = 0; i < 4; i++)
                    {
                        var quest = RoguegardSettings.DungeonQuestGenerator.GenerateQuest(RogueRandom.Primary);
                        questList.Add(quest);
                    }

                    view.Show(questList, manager)
                    ?
                    .NameFrom(quest => quest.Caption)

                    .VarOnce(out var nextScreen, new QuestSummaryScreen())
                    .OnClick((quest, manager) => manager.PushScreen(nextScreen, Arg.Self, other: quest))

                    .Build();
                };
            }

            private class QuestSummaryScreen : RogueListuiScreen
            {
                public QuestSummaryScreen()
                {
                    OnOpenScreen += (manager) =>
                    {
                        var quest = (DungeonQuest)Arg.Arg.Other;

                        manager.Summary.SetQuest(Arg.Self, quest, true, manager);
                        manager.Summary.Show();
                    };
                }
            }
        }
    }
}
