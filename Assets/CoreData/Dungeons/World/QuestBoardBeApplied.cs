using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.Device;
using Roguegard.CharacterCreation;

namespace Roguegard
{
    public class QuestBoardBeApplied : ReferableScript, IApplyRogueMethod
    {
        private QuestBoardBeApplied() { }

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

        public class RogueMenu : RogueMenuScreen
        {
            private static readonly List<DungeonQuest> elms = new List<DungeonQuest>();
            private static readonly QuestViewMenu nextMenu = new QuestViewMenu();

            private readonly ScrollViewTemplate<DungeonQuest, MMgr, MArg> view = new()
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

                view.ShowTemplate(elms, manager, arg)
                    ?
                    .ElementNameFrom((quest, manager, arg) =>
                    {
                        return quest.Caption;
                    })

                    .OnClickElement((quest, manager, arg) =>
                    {
                        manager.PushMenuScreen(nextMenu, arg.Self, other: quest);
                    })

                    .Build();
            }

            private class QuestViewMenu : RogueMenuScreen
            {
                private readonly DialogViewTemplate<MMgr, MArg> view = new()
                {
                };

                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    var quest = (DungeonQuest)arg.Arg.Other;

                    view.ShowTemplate(quest.Caption, manager, arg)
                        ?.Build();
                }
            }
        }
    }
}
