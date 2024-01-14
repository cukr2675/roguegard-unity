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

        public SkillsMenu(CaptionWindow captionWindow)
        {
            var commandMenu = new SkillCommandMenu();
            {
                var use = new UseMenu();
                use.caption = captionWindow;
                use.action = (IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg) =>
                {
                    // スクロール位置を記憶する。違うキャラのインベントリを開いたときリセットされる
                    use.selfIndex = root.Get(DeviceKw.MenuScroll).GetPosition();
                    use.prevSelf = self;
                };
                use.exitChoice = new ExitModelsMenuChoice(use.action);
                Use = use;
            }
        }

        /// <summary>
        /// 使うスキルを選択するメニュー
        /// </summary>
        private class UseMenu : IModelsMenu, IModelsMenuItemController, ISkillMenuItemController
        {
            private readonly SkillCommandMenu menu = new SkillCommandMenu();
            public IModelsMenuChoice exitChoice;
            public ModelsMenuAction action;
            public CaptionWindow caption;

            public RogueObj prevSelf;
            public float selfIndex;
            private List<object> models = new List<object>();

            RogueObj ISkillMenuItemController.Obj => prevSelf;

            void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                models.Clear();
                for (int i = 0; i < self.Main.Skills.Count; i++)
                {
                    models.Add(self.Main.Skills[i]);
                }
                var position = self == prevSelf ? selfIndex : 0f;
                prevSelf = self;
                var openArg = new RogueMethodArgument(targetObj: self, vector: new Vector2(position, 0f));
                var scroll = (ScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, models, root, self, user, openArg);
                scroll.ShowExitButton(exitChoice);
                caption.ShowCaption("スキル：");
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var skill = (ISkill)model;
                return skill.Name;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                action(root, self, user, arg);
                var skill = (ISkill)model;
                if (skill == null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                caption.ShowCaption(skill);
                var openArg = new RogueMethodArgument(other: skill);
                root.OpenMenuAsDialog(menu, self, null, openArg, RogueMethodArgument.Identity);
            }
        }
    }
}
