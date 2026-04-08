using Roguegard;
using Roguegard.Device;
using System.Collections.Generic;

namespace RoguegardUnity
{
    // スキル選択メニュー。
    public class SkillsMenu
    {
        public RogueListuiScreen Use { get; }

        public SkillsMenu()
        {
            Use = new UseMenu();
        }

        /// <summary>
        /// 使うスキルを選択するメニュー
        /// </summary>
        private class UseMenu : RogueListuiScreen
        {
            private readonly RogueScrollMenuViewData<ISkill> view = new()
            {
                Title = ":Skills",
            };

            public UseMenu()
            {
                var list = new List<ISkill>();

                OnOpenScreen += (manager) =>
                {
                    var skills = Arg.Self.Main.Skills;
                    list.Clear();
                    for (int i = 0; i < skills.Count; i++)
                    {
                        list.Add(skills[i]);
                    }

                    view.Show(list, manager)
                    ?
                    .InfoFrom((skill, manager) =>
                    {
                        var obj = Arg.Arg.TargetObj ?? Arg.Self;

                        var requiredMp = StatsEffectedValues.GetRequiredMp(obj, skill.RequiredMp);
                        return ((skill, obj), null, $"{requiredMp} MP");
                    })

                    .VarOnce(out var menu, new SkillCommandMenuScreen())
                    .OnClick((skill, manager) =>
                    {
                        // 選択したスキルの情報と選択肢を表示する
                        manager.PushScreen(menu, Arg.Self, other: skill);
                    })

                    .Build();
                };
            }
        }
    }
}
