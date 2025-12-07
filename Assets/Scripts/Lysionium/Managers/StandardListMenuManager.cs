using UnityEngine;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/LUI Standard List Menu Manager")]
    public class StandardListMenuManager : StandardListMenuManager<StandardListMenuManager, IListMenuArg>
    {
        public void Initialize() => CommonInit();
    }

    [RequireComponent(typeof(StandardSubviewTable))]
    public abstract class StandardListMenuManager<TMgr, TArg> : MonoBehaviour, IListMenuManager, IDefaultSubviewTable, IMenuScreenListMenuManager<TMgr, TArg>
        where TMgr : StandardListMenuManager<TMgr, TArg>
        where TArg : IListMenuArg
    {
        public StandardSubviewTable StandardSubviewTable { get; private set; }

        public event System.Action OnError;
        public event System.Action OnDone;
        public event System.Action OnUnload;

        private readonly MenuScreenStack<TMgr, TArg> stack = new();
        private MenuScreenStack<TMgr, TArg>.StackItem reservedMenu;

        public bool ShowsMenuScreen => stack.Count >= 1;

        public bool IsDone { get; private set; }

        public virtual ISelectOption BackOption { get; protected set; }
            = SelectOption.Create<TMgr, TArg>("Back", (manager, arg) => manager.PopMenuScreen(), "Cancel click:Cancel");

        public virtual ISelectOption ErrorOption { get; protected set; }
            = SelectOption.Create<IListMenuManager, IListMenuArg>("<#F00>ERROR", delegate { }, "Cancel");

        /// <summary>
        /// この値が true の間は予約されたメニューを表示しない。遷移アニメーション用
        /// </summary>
        protected virtual bool HasManagerLock => StandardSubviewTable.HasManagerLock;

        public IListHandlerSubview Scroll => StandardSubviewTable.Scroll;
        public IListHandlerSubview Widgets => StandardSubviewTable.Widgets;
        public IMessageBoxSubview LongMessage => StandardSubviewTable.LongMessage;
        public IListHandlerSubview BackAnchor => StandardSubviewTable.BackAnchor;
        public IListHandlerSubview ForwardAnchor => StandardSubviewTable.ForwardAnchor;
        public IListHandlerSubview PrimaryCommand => StandardSubviewTable.PrimaryCommand;
        public IListHandlerSubview CaptionBox => StandardSubviewTable.CaptionBox;
        public IListHandlerSubview SecondaryCommand => StandardSubviewTable.SecondaryCommand;
        public IListHandlerSubview Dialog => StandardSubviewTable.Dialog;
        public IColorPickerSubview ColorPicker => StandardSubviewTable.ColorPicker;
        public IMessageBoxSubview MessageBox => StandardSubviewTable.MessageBox;
        public IListHandlerSubview FadeMask => StandardSubviewTable.FadeMask;
        public IListHandlerSubview Overlay => StandardSubviewTable.Overlay;
        public IMessageBoxSubview SpeechBox => StandardSubviewTable.SpeechBox;
        public IListHandlerSubview Choices => StandardSubviewTable.Choices;
        public IListHandlerSubview DropdownList => throw new System.NotImplementedException();
        public IListHandlerSubview DropdownGrid => throw new System.NotImplementedException();

        protected void CommonInit()
        {
            StandardSubviewTable = GetComponent<StandardSubviewTable>();
            StandardSubviewTable.CommonInit();
            HideAll();
        }

        protected virtual void OnDestroy()
        {
            OnUnload?.Invoke();
            OnUnload = null;
        }

        // アニメーションが再生されるのを待機するため Update ではなく LateUpdate にする
        protected virtual void LateUpdate()
        {
            if (reservedMenu == null || HasManagerLock) return;

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

        protected virtual void BlockAll()
        {
            foreach (var subview in StandardSubviewTable.Subviews.Values)
            {
                subview.SetInteractable(false);
            }
        }

        public virtual void HideAll(bool back = false)
        {
            foreach (var subview in StandardSubviewTable.Subviews.Values)
            {
                subview.Hide(back);
            }
        }

        public virtual string Localize(string text) => text?.Normalize(System.Text.NormalizationForm.FormC); // TextMeshPro のために NFD を NFC に正規化する

        public virtual T Localize<T>(T obj) => obj;

        /// <summary>
        /// メニューを指定の画面へ進める
        /// </summary>
        public virtual void PushMenuScreen(IMenuScreen<TMgr, TArg> menuScreen, TArg arg)
        {
            menuScreen.CloseScreenView((TMgr)this, false);
            BlockAll();
            reservedMenu = stack.Push(menuScreen, arg);
        }

        public void PushInitialMenuScreen(IMenuScreen<TMgr, TArg> menu, TArg arg, bool enableTouchMask = true)
        {
            stack.Clear();
            PushMenuScreen(menu, arg);
            StandardSubviewTable.SetBlocker(enableTouchMask);
        }

        /// <summary>
        /// メニュー画面を指定の回数戻る
        /// </summary>
        public void PopMenuScreen(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (stack.Count == 0) break;

                var item = stack.Pop();
                item.MenuScreen.CloseScreenView((TMgr)this, true);
            }
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
            StandardSubviewTable.SetBlocker(false);
            IsDone = true;
            OnDone?.Invoke();
        }

        public void ResetDone()
        {
            IsDone = false;
        }
    }
}
