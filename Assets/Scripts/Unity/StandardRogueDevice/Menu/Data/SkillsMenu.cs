using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    // スキル選択メニュー。
    public class SkillsMenu
    {
        public RogueMenuScreen Use { get; }

        public SkillsMenu()
        {
            Use = new UseMenu();
        }

        /// <summary>
        /// 使うスキルを選択するメニュー
        /// </summary>
        private class UseMenu : RogueMenuScreen
        {
            private readonly ScrollViewTemplate<ISkill, RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
                Title = ":Skills",
            };

            private readonly List<ISkill> list = new List<ISkill>();
            private readonly SkillCommandMenu menu = new SkillCommandMenu();

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                var skills = arg.Self.Main.Skills;
                list.Clear();
                for (int i = 0; i < skills.Count; i++)
                {
                    list.Add(skills[i]);
                }

                view.Show(list, manager, arg)
                    ?.ElementNameGetter((skill, manager, arg) =>
                    {
                        return skill.Name;
                    })
                    .OnClickElement((skill, manager, arg) =>
                    {
                        // 選択したスキルの情報と選択肢を表示する
                        manager.PushMenuScreen(menu, other: skill);
                    })
                    .Build();
            }
        }
    }
}
