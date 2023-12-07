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

        public class RogueMenu : IModelsMenu
        {
            private static readonly ItemController itemController = new ItemController();
            private static readonly List<object> models = new List<object>();

            public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
            {
                models.Clear();
                for (int i = 0; i < 4; i++)
                {
                    var quest = RoguegardSettings.DungeonQuestGenerator.GenerateQuest(RogueRandom.Primary);
                    models.Add(quest);
                }

                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(itemController, models, root, player, null, RogueMethodArgument.Identity);
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

            private class ItemController : IModelsMenuItemController
            {
                private static readonly QuestViewMenu nextMenu = new QuestViewMenu();

                public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return ((DungeonQuest)model).Caption;
                }

                public void Activate(object model, IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
                {
                    var quest = (DungeonQuest)model;

                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.OpenMenu(nextMenu, player, null, new(other: quest), RogueMethodArgument.Identity);
                }
            }

            private class QuestViewMenu : IModelsMenu
            {
                public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
                {
                    var quest = (DungeonQuest)arg.Other;
                    var summary = (IDungeonQuestMenuView)root.Get(DeviceKw.MenuSummary);
                    summary.OpenView(ChoicesModelsMenuItemController.Instance, Spanning<object>.Empty, root, player, null, RogueMethodArgument.Identity);
                    summary.SetQuest(player, quest, true);
                }
            }
        }
    }
}
