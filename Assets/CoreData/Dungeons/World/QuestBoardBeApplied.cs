using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public class RogueMenu : IListMenu
        {
            private static readonly Presenter presenter = new Presenter();
            private static readonly List<object> elms = new List<object>();

            public void OpenMenu(IListMenuManager manager, RogueObj player, RogueObj user, in RogueMethodArgument arg)
            {
                elms.Clear();
                for (int i = 0; i < 4; i++)
                {
                    var quest = RoguegardSettings.DungeonQuestGenerator.GenerateQuest(RogueRandom.Primary);
                    elms.Add(quest);
                }

                var scroll = manager.GetView(DeviceKw.MenuScroll);
                scroll.OpenView(presenter, elms, manager, player, null, RogueMethodArgument.Identity);
                ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
            }

            private class Presenter : IElementPresenter
            {
                private static readonly QuestViewMenu nextMenu = new QuestViewMenu();

                public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return ((DungeonQuest)element).Caption;
                }

                public void ActivateItem(object element, IListMenuManager manager, RogueObj player, RogueObj user, in RogueMethodArgument arg)
                {
                    var quest = (DungeonQuest)element;

                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    manager.OpenMenu(nextMenu, player, null, new(other: quest));
                }
            }

            private class QuestViewMenu : IListMenu
            {
                public void OpenMenu(IListMenuManager manager, RogueObj player, RogueObj user, in RogueMethodArgument arg)
                {
                    var quest = (DungeonQuest)arg.Other;
                    var summary = (IDungeonQuestMenuView)manager.GetView(DeviceKw.MenuSummary);
                    summary.OpenView(SelectOptionPresenter.Instance, Spanning<object>.Empty, manager, player, null, RogueMethodArgument.Identity);
                    summary.SetQuest(player, quest, true);
                }
            }
        }
    }
}
