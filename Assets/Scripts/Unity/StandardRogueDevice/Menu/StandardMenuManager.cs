//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using UnityEngine.UI;
//using Roguegard;
//using Roguegard.Device;

//namespace RoguegardUnity
//{
//    internal class StandardMenuManager : IListMenuManager
//    {
//        private readonly Image touchMask;
//        private readonly MessageController messageController;
//        private readonly StatsWindow statsWindow;
//        private readonly Dictionary<IKeyword, ElementsView> table;
//        private readonly Stack<StackItem> stack;

//        private bool currentMenuIsDialog;

//        public ListMenuEventManager EventManager { get; }

//        public StatsWindow Stats => statsWindow;

//        public bool Any => stack.Count >= 1;
//        public bool IsDone { get; private set; }

//        public StandardMenuManager(
//            Image touchMask, MessageController messageController, StatsWindow statsWindow,
//            SoundController soundController, IReadOnlyDictionary<IKeyword, ElementsView> table)
//        {
//            this.touchMask = touchMask;
//            this.messageController = messageController;
//            this.statsWindow = statsWindow;
//            this.table = new Dictionary<IKeyword, ElementsView>(table);
//            EventManager = new ListMenuEventManager(messageController, soundController);
//            stack = new Stack<StackItem>();
//        }

//        private void OpenMenu(IListMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//        {
//            HideAll();
//            menu.OpenMenu(this, self, user, arg);

//            currentMenuIsDialog = false;

//            var stackItem = new StackItem();
//            stackItem.Set(menu, self, user, arg);
//            stack.Push(stackItem);
//            touchMask.raycastTarget = true;
//        }

//        public void OpenInitialMenu(
//            IListMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg, bool enableTouchMask = true)
//        {
//            stack.Clear();
//            OpenMenu(menu, self, user, arg);

//            if (!enableTouchMask) { touchMask.raycastTarget = false; }
//        }

//        public void ResetDone()
//        {
//            IsDone = false;
//        }

//        private void HideAll()
//        {
//            messageController.ShowMessage(false);
//            statsWindow.Show(false);
//            foreach (var item in table)
//            {
//                MenuController.Show(item.Value.CanvasGroup, false);
//            }
//        }

//        public void Done()
//        {
//            stack.Clear();
//            currentMenuIsDialog = false;

//            HideAll();
//            touchMask.raycastTarget = false;

//            IsDone = true;
//        }

//        private void Back()
//        {
//            HideAll();

//            if (currentMenuIsDialog)
//            {
//                // ダイアログから戻るときはダイアログを閉じるようにする
//                currentMenuIsDialog = false;
//            }
//            else
//            {
//                // ダイアログでない場合は、メニューの階層を一つ戻す（パンくずリストをひとつ戻るイメージ）
//                stack.Pop();
//            }

//            if (stack.TryPeek(out var stackItem))
//            {
//                // ひとつ前のメニューが存在する場合、そのメニューを開きなおす
//                stackItem.Menu.OpenMenu(this, stackItem.Self, stackItem.User, stackItem.Arg);
//            }
//            else
//            {
//                // メニューがない場合は終了する
//                Done();
//            }
//        }

//        void IListMenuManager.Back() => Back();

//        IElementsView IListMenuManager.GetView(IKeyword keyword) => table[keyword];

//        void IListMenuManager.OpenMenu(IListMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//            => OpenMenu(menu, self, user, arg);

//        void IListMenuManager.OpenMenuAsDialog(IListMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//        {
//            menu.OpenMenu(this, self, user, arg);
//            currentMenuIsDialog = true;
//        }

//        void IListMenuManager.Reopen()
//        {
//            currentMenuIsDialog = true;
//            Back();
//        }

//        void IListMenuManager.AddInt(IKeyword keyword, int integer) => EventManager.Add(keyword, integer: integer);
//        void IListMenuManager.AddFloat(IKeyword keyword, float number) => EventManager.Add(keyword, number: number);
//        void IListMenuManager.AddObject(IKeyword keyword, object obj) => EventManager.Add(keyword, obj: obj);
//        void IListMenuManager.AddWork(IKeyword keyword, in RogueCharacterWork work) => throw new System.NotSupportedException();

//        private class StackItem
//        {
//            public IListMenu Menu { get; private set; }

//            public RogueObj Self { get; private set; }

//            public RogueObj User { get; private set; }

//            public RogueMethodArgument Arg { get; set; }

//            public void Set(IListMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//            {
//                Menu = menu;
//                Self = self;
//                User = user;
//                Arg = arg;
//            }
//        }
//    }
//}
