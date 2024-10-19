using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Standard List Menu Manager")]
    public class StandardListMenuManager : StandardListMenuManager<StandardListMenuManager, IListMenuArg>
    {
    }

    [RequireComponent(typeof(StandardSubViewTable))]
    public abstract class StandardListMenuManager<TMgr, TArg> : MonoBehaviour, IListMenuManager
        where TMgr : StandardListMenuManager<TMgr, TArg>
        where TArg : IListMenuArg
    {
        public StandardSubViewTable StandardSubViewTable { get; private set; }

        private readonly MenuScreenStack<TMgr, TArg> stack = new();
        private MenuScreenStack<TMgr, TArg>.StackItem reservedMenu;

        protected int StackCount => stack.Count;

        public bool IsDone { get; private set; }

        public virtual string BackName => "<";

        private bool IsLocked
        {
            get
            {
                foreach (var subView in StandardSubViewTable.SubViews.Values)
                {
                    if (subView.HasManagerLock) return true;
                }
                return false;
            }
        }

        public void Initialize()
        {
            StandardSubViewTable = GetComponent<StandardSubViewTable>();
            StandardSubViewTable.Initialize();
            HideAll();
        }

        // �A�j���[�V�������Đ������̂�ҋ@���邽�� Update �ł͂Ȃ� LateUpdate �ɂ���
        private void LateUpdate()
        {
            if (reservedMenu == null || IsLocked) return;

            try
            {
                reservedMenu.MenuScreen.OpenScreen((TMgr)this, reservedMenu.Arg);
            }
            catch (System.Exception)
            {
                Done(); // ��O�������̓��j���[�����i����s�\�ɂȂ�\�������邽�߁j
                throw;
            }
            finally
            {
                reservedMenu = null;
            }
        }

        public IElementsSubView GetSubView(string subViewName)
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

        private void BlockAll()
        {
            foreach (var subView in StandardSubViewTable.SubViews.Values)
            {
                subView.SetBlock(true);
            }
        }

        public void PushMenuScreen(MenuScreen<TMgr, TArg> menuScreen, TArg arg)
        {
            menuScreen.CloseScreen((TMgr)this, false);
            BlockAll();
            reservedMenu = stack.Push(menuScreen, arg);
        }

        void IListMenuManager.PushMenuScreen(IMenuScreen menuScreen, IListMenuArg arg)
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

        public virtual void HandleClickBack()
        {
            var popItem = stack.Pop();
            popItem.MenuScreen.CloseScreen((TMgr)this, true);

            if (stack.TryPeek(out var stackItem))
            {
                // �ЂƂO�̃��j���[�����݂���ꍇ�A���̃��j���[���J���Ȃ���
                BlockAll();
                reservedMenu = stackItem;
            }
            else
            {
                // ���j���[���Ȃ��ꍇ�͏I������
                Done();
            }
        }

        public virtual void HandleClickError()
        {
        }

        public void Reopen()
        {
            //currentMenuIsDialog = true;
            //Back();
        }

        public void Done()
        {
            stack.Clear();
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
