using Lysionium.Views;
using System.Text;
using UnityEngine;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/LUI Standard List Menu Manager")]
    public class StandardListMenuManager : StandardListMenuManager<StandardListMenuManager, IListMenuArg>
    {
        public void Initialize() => CommonInit();
    }

    public abstract class StandardListMenuManager<TMgr> : StandardListMenuManager<TMgr, IListMenuArg>
        where TMgr : StandardListMenuManager<TMgr, IListMenuArg>
    { }

    [RequireComponent(typeof(DefaultSubviewTable))]
    public abstract class StandardListMenuManager<TMgr, TArg> : MonoBehaviour, IMenuScreenListMenuManager<TMgr, TArg>, IDefaultSubviewTable
        where TMgr : StandardListMenuManager<TMgr, TArg>
        where TArg : IListMenuArg
    {
        private DefaultSubviewTable defaultSubviewTable;

        public event System.Action OnError;
        public event System.Action OnUnload;

        private readonly MenuScreenStack<TMgr, TArg> stack = new();
        private MenuScreenStack<TMgr, TArg>.StackItem reservedMenu;

        public bool ShowsMenuScreen => stack.Count >= 1;

        public virtual ISelectOption BackOption { get; protected set; }
            = SelectOption.Create<TMgr, TArg>("Back", (manager, arg) => manager.PopMenuScreen(), "Cancel click:Cancel");

        public virtual ISelectOption ErrorOption { get; protected set; }
            = SelectOption.Create<IListMenuManager, IListMenuArg>("<#F00>ERROR", delegate { }, "Cancel");

        /// <summary>
        /// この値が true の間は予約されたメニューを表示しない。遷移アニメーション用
        /// </summary>
        protected virtual bool HasManagerLock => defaultSubviewTable.HasManagerLock;

        public IListHandlerSubview Scroll => defaultSubviewTable.Scroll;
        public IListHandlerSubview Widgets => defaultSubviewTable.Widgets;
        public IMessageBoxSubview LongMessage => defaultSubviewTable.LongMessage;
        public IListHandlerSubview BackAnchor => defaultSubviewTable.BackAnchor;
        public IListHandlerSubview ForwardAnchor => defaultSubviewTable.ForwardAnchor;
        public IListHandlerSubview PrimaryCommand => defaultSubviewTable.PrimaryCommand;
        public IListHandlerSubview CaptionBox => defaultSubviewTable.CaptionBox;
        public IListHandlerSubview SecondaryCommand => defaultSubviewTable.SecondaryCommand;
        public IListHandlerSubview Dialog => defaultSubviewTable.Dialog;
        public IColorPickerSubview ColorPicker => defaultSubviewTable.ColorPicker;
        public IMessageBoxSubview MessageBox => defaultSubviewTable.MessageBox;
        public IListHandlerSubview FadeMask => defaultSubviewTable.FadeMask;
        public IListHandlerSubview Overlay => defaultSubviewTable.Overlay;
        public IMessageBoxSubview SpeechBox => defaultSubviewTable.SpeechBox;
        public IListHandlerSubview Choices => defaultSubviewTable.Choices;
        public IListHandlerSubview DropdownList => throw new System.NotImplementedException();
        public IListHandlerSubview DropdownGrid => throw new System.NotImplementedException();

        protected void CommonInit()
        {
            defaultSubviewTable = GetComponent<DefaultSubviewTable>();
            defaultSubviewTable.CommonInit();
            HideAll();
        }

        protected virtual void OnDestroy()
        {
            OnUnload?.Invoke();
            if (OnUnload != null)
            {
                Debug.LogWarning(
                    $"{name}.OnDestroy() 時にイベントが購読解除されていません。これらは自動的に解除されます: " +
                    $"({string.Join(", ", (object[])OnUnload.GetInvocationList())})");
            }
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
            catch
            {
                reservedMenu = null;
                OnError?.Invoke();
                throw;
            }
        }

        protected virtual void BlockAll()
        {
            for (int i = 0; i < defaultSubviewTable.Subviews.Count; i++)
            {
                defaultSubviewTable.Subviews[i].SetInteractable(false);
            }
        }

        public virtual void HideAll(bool back = false)
        {
            for (int i = 0; i < defaultSubviewTable.Subviews.Count; i++)
            {
                defaultSubviewTable.Subviews[i].Hide(back);
            }
        }

        public virtual string Localize(string text) => text?.Normalize(NormalizationForm.FormC); // TextMeshPro のために NFD を NFC に正規化する

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
            defaultSubviewTable.SetBlocker(enableTouchMask);
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
                defaultSubviewTable.SetBlocker(false);
            }
        }

        /// <summary>
        /// メニュー画面をすべて閉じる
        /// </summary>
        protected void Clear()
        {
            stack.Clear();
            reservedMenu = null;
            HideAll();
            defaultSubviewTable.SetBlocker(false);
        }
    }
}
