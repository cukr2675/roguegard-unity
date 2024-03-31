using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class StandardMenuRoot : IModelsMenuRoot
    {
        private readonly Image touchMask;
        private readonly MessageController messageController;
        private readonly StatsWindow statsWindow;
        private readonly Dictionary<IKeyword, ModelsMenuView> table;
        private readonly Stack<StackItem> stack;

        private bool currentMenuIsDialog;

        public ModelsMenuEventManager EventManager { get; }

        public StatsWindow Stats => statsWindow;

        public bool Any => stack.Count >= 1;
        public bool IsDone { get; private set; }

        public StandardMenuRoot(
            Image touchMask, MessageController messageController, StatsWindow statsWindow,
            SoundController soundController, IReadOnlyDictionary<IKeyword, ModelsMenuView> table)
        {
            this.touchMask = touchMask;
            this.messageController = messageController;
            this.statsWindow = statsWindow;
            this.table = new Dictionary<IKeyword, ModelsMenuView>(table);
            EventManager = new ModelsMenuEventManager(messageController, soundController);
            stack = new Stack<StackItem>();
        }

        private void OpenMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            HideAll();
            menu.OpenMenu(this, self, user, arg);

            currentMenuIsDialog = false;

            var stackItem = new StackItem();
            stackItem.Set(menu, self, user, arg);
            stack.Push(stackItem);
            touchMask.raycastTarget = true;
        }

        public void OpenInitialMenu(
            IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg, bool enableTouchMask = true)
        {
            stack.Clear();
            OpenMenu(menu, self, user, arg);

            if (!enableTouchMask) { touchMask.raycastTarget = false; }
        }

        public void ResetDone()
        {
            IsDone = false;
        }

        private void HideAll()
        {
            messageController.ShowMessage(false);
            statsWindow.Show(false);
            foreach (var item in table)
            {
                MenuController.Show(item.Value.CanvasGroup, false);
            }
        }

        public void Done()
        {
            stack.Clear();
            currentMenuIsDialog = false;

            HideAll();
            touchMask.raycastTarget = false;

            IsDone = true;
        }

        private void Back()
        {
            HideAll();

            if (currentMenuIsDialog)
            {
                // ダイアログから戻るときはダイアログを閉じるようにする
                currentMenuIsDialog = false;
            }
            else
            {
                // ダイアログでない場合は、メニューの階層を一つ戻す（パンくずリストをひとつ戻るイメージ）
                stack.Pop();
            }

            if (stack.TryPeek(out var stackItem))
            {
                // ひとつ前のメニューが存在する場合、そのメニューを開きなおす
                stackItem.Menu.OpenMenu(this, stackItem.Self, stackItem.User, stackItem.Arg);
            }
            else
            {
                // メニューがない場合は終了する
                Done();
            }
        }

        void IModelsMenuRoot.Back() => Back();

        IModelsMenuView IModelsMenuRoot.Get(IKeyword keyword) => table[keyword];

        void IModelsMenuRoot.OpenMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            => OpenMenu(menu, self, user, arg);

        void IModelsMenuRoot.OpenMenuAsDialog(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            menu.OpenMenu(this, self, user, arg);
            currentMenuIsDialog = true;
        }

        void IModelsMenuRoot.Reopen(RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            currentMenuIsDialog = true;
            Back();
        }

        void IModelsMenuRoot.AddInt(IKeyword keyword, int integer) => EventManager.Add(keyword, integer: integer);
        void IModelsMenuRoot.AddFloat(IKeyword keyword, float number) => EventManager.Add(keyword, number: number);
        void IModelsMenuRoot.AddObject(IKeyword keyword, object obj) => EventManager.Add(keyword, obj: obj);
        void IModelsMenuRoot.AddWork(IKeyword keyword, in RogueCharacterWork work) => throw new System.NotSupportedException();

        private class StackItem
        {
            public IModelsMenu Menu { get; private set; }

            public RogueObj Self { get; private set; }

            public RogueObj User { get; private set; }

            public RogueMethodArgument Arg { get; set; }

            public void Set(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                Menu = menu;
                Self = self;
                User = user;
                Arg = arg;
            }
        }
    }
}
