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
        public IListMenu Use { get; }

        public SkillsMenu()
        {
            Use = new UseMenu();
        }

        /// <summary>
        /// 使うスキルを選択するメニュー
        /// </summary>
        private class UseMenu : BaseScrollListMenu<ISkill>, ISkillMenuItemController
        {
            protected override string MenuName => ":Skills";

            private readonly List<ISkill> elms = new List<ISkill>();
            private readonly SkillCommandMenu menu = new SkillCommandMenu();

            protected override Spanning<ISkill> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                elms.Clear();
                for (int i = 0; i < self.Main.Skills.Count; i++)
                {
                    elms.Add(self.Main.Skills[i]);
                }
                return elms;
            }

            protected override string GetItemName(ISkill skill, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "???";
            }

            protected override void ActivateItem(ISkill skill, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // 選択したスキルの情報と選択肢を表示する
                manager.OpenMenuAsDialog(menu, self, null, new(other: skill));
            }
        }
    }
}
