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
            private readonly RogueScrollViewTemplate<ISkill> view = new()
            {
                Title = ":Skills",
            };

            private readonly List<ISkill> list = new();
            private readonly SkillCommandMenu menu = new();

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var skills = arg.Self.Main.Skills;
                list.Clear();
                for (int i = 0; i < skills.Count; i++)
                {
                    list.Add(skills[i]);
                }

                view.ShowTemplate(list, manager, arg)
                    ?
                    .ElementInfoFrom((skill, manager, arg) =>
                    {
                        var obj = arg.Arg.TargetObj ?? arg.Self;

                        var requiredMP = StatsEffectedValues.GetRequiredMP(obj, skill.RequiredMP);
                        return (skill, null, $"{requiredMP} MP");
                    })

                    .OnClickElement((skill, manager, arg) =>
                    {
                        // 選択したスキルの情報と選択肢を表示する
                        manager.PushMenuScreen(menu, arg.Self, other: skill);
                    })

                    .Build();
            }
        }
    }
}
