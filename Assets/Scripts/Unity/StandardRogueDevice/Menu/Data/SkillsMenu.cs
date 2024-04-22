using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    // スキル選択メニュー。
    public class SkillsMenu
    {
        public IModelsMenu Use { get; }

        public SkillsMenu()
        {
            Use = new UseMenu();
        }

        /// <summary>
        /// 使うスキルを選択するメニュー
        /// </summary>
        private class UseMenu : BaseScrollModelsMenu<ISkill>, ISkillMenuItemController
        {
            protected override string MenuName => ":Skills";

            private readonly List<ISkill> models = new List<ISkill>();
            private readonly SkillCommandMenu menu = new SkillCommandMenu();

            protected override Spanning<ISkill> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                models.Clear();
                for (int i = 0; i < self.Main.Skills.Count; i++)
                {
                    models.Add(self.Main.Skills[i]);
                }
                return models;
            }

            protected override string GetItemName(ISkill skill, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "???";
            }

            protected override void ActivateItem(ISkill skill, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // 選択したスキルの情報と選択肢を表示する
                root.OpenMenuAsDialog(menu, self, null, new(other: skill));
            }
        }
    }
}
