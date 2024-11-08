using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Standard List Menu Manager")]
    public class StandardListMenuManager : StandardListMenuManager<StandardListMenuManager, IListMenuArg>
    {
        public new void Initialize() => base.Initialize();
    }

    [RequireComponent(typeof(StandardSubViewTable))]
    public abstract class StandardListMenuManager<TMgr, TArg> : MonoBehaviour, IListMenuManager
        where TMgr : StandardListMenuManager<TMgr, TArg>
        where TArg : IListMenuArg
    {
        public StandardSubViewTable StandardSubViewTable { get; private set; }

        public event System.Action OnError;

        private readonly MenuScreenStack<TMgr, TArg> stack = new();
        private MenuScreenStack<TMgr, TArg>.StackItem reservedMenu;

        public bool ShowsMenuScreen => stack.Count >= 1;

        public bool IsDone { get; private set; }

        public virtual ISelectOption BackOption { get; protected set; }
            = SelectOption.Create<TMgr, TArg>("Back", (manager, arg) => manager.Back(), "Cancel");

        public virtual ISelectOption ErrorOption { get; protected set; }
            = SelectOption.Create<IListMenuManager, IListMenuArg>("<#F00>ERROR", delegate { }, "Cancel");

        protected void Initialize()
        {
            StandardSubViewTable = GetComponent<StandardSubViewTable>();
            StandardSubViewTable.Initialize();
            HideAll();
        }

        // アニメーションが再生されるのを待機するため Update ではなく LateUpdate にする
        private void LateUpdate()
        {
            if (reservedMenu == null || StandardSubViewTable.HasManagerLock) return;

            try
            {
                reservedMenu.MenuScreen.OpenScreen((TMgr)this, reservedMenu.Arg);
                reservedMenu = null;
            }
            catch (System.Exception)
            {
                reservedMenu = null;
                OnError?.Invoke();
                throw;
            }
        }

        public virtual IElementsSubView GetSubView(string subViewName)
        {
            return StandardSubViewTable.SubViews[subViewName];
        }

        public virtual void HideAll(bool back = false)
        {
            foreach (var subView in StandardSubViewTable.SubViews.Values)
            {
                subView.Hide(back);
            }
        }

        public virtual string Localize(string text) => text;

        public virtual T Localize<T>(T obj) => obj;

        private void BlockAll()
        {
            foreach (var subView in StandardSubViewTable.SubViews.Values)
            {
                subView.SetBlock(true);
            }
        }

        /// <summary>
        /// メニューを指定の画面へ進める
        /// </summary>
        public virtual void PushMenuScreen(MenuScreen<TMgr, TArg> menuScreen, TArg arg)
        {
            menuScreen.CloseScreen((TMgr)this, false);
            BlockAll();
            reservedMenu = stack.Push(menuScreen, arg);
        }

        void IListMenuManager.PushMenuScreenFromExtension(object menuScreen, IListMenuArg arg)
        {
            if (LMFAssert.Type<MenuScreen<TMgr, TArg>>(menuScreen, out var tMenuScreen) ||
                LMFAssert.Type<TArg>(arg, out var tArg)) throw new System.ArgumentException();

            PushMenuScreen(tMenuScreen, tArg);
        }

        public void PushInitialMenuScreen(MenuScreen<TMgr, TArg> menu, TArg arg, bool enableTouchMask = true)
        {
            stack.Clear();
            PushMenuScreen(menu, arg);
            StandardSubViewTable.SetBlocker(enableTouchMask);
        }

        /// <summary>
        /// メニュー画面を指定の回数戻る
        /// </summary>
        public void Back(int count = 1)
        {
            MenuScreenStack<TMgr, TArg>.StackItem lastItem = null;
            for (int i = 0; i < count; i++)
            {
                if (stack.Count == 0) break;

                lastItem = stack.Pop();
            }
            lastItem?.MenuScreen.CloseScreen((TMgr)this, true);
            Reopen();
        }

        /// <summary>
        /// 現在表示されているメニュー画面を更新する
        /// </summary>
        public void Reopen()
        {
            if (stack.TryPeek(out var stackItem))
            {
                // ひとつ前のメニューが存在する場合、そのメニューを開きなおす
                BlockAll();
                reservedMenu = stackItem;
            }
            else
            {
                // メニューがない場合は終了する
                Done();
            }
        }

        /// <summary>
        /// メニュー画面をすべて閉じる
        /// </summary>
        public void Done()
        {
            stack.Clear();
            reservedMenu = null;
            HideAll();
            StandardSubViewTable.SetBlocker(false);
            IsDone = true;
        }

        public void ResetDone()
        {
            IsDone = false;
        }
    }
}
