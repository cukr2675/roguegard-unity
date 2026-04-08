using Lysionium.Views;
using System.Text;
using UnityEngine;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/LUI Standard List-UI Manager")]
    public class StandardListuiManager : StandardListuiManager<StandardListuiManager>
    {
        public void Initialize() => CommonInit();
    }

    [RequireComponent(typeof(DefaultSubviewTable))]
    public abstract class StandardListuiManager<TMgr>
        : MonoBehaviour, IListuiScreenManager<TMgr>, IBackOptionProviderListuiManager<TMgr>, IDefaultSubviewTable
        where TMgr : StandardListuiManager<TMgr>
    {
        private DefaultSubviewTable defaultSubviewTable;

        public event System.Action OnError;
        public event System.Action OnUnload;

        private readonly ListuiScreenStack<TMgr> stack = new();
        private ListuiScreenStack<TMgr>.StackItem reservedScreen;

        public virtual ISelectOption<TMgr> BackOption { get; protected set; }
            = SelectOption.Create<TMgr>("Back", m => m.PopScreen(), "Cancel click:Cancel");

        public virtual ISelectOption<IListuiManager> ErrorOption { get; protected set; }
            = SelectOption.Create<IListuiManager>("<#F00>ERROR", delegate { }, "Cancel");

        /// <summary>
        /// この値が true の間は予約されたメニューを表示しない。遷移アニメーション用
        /// </summary>
        protected virtual bool HasManagerLock => defaultSubviewTable.HasManagerLock;

        public IListHandlerSubview PlayingHud => defaultSubviewTable.PlayingHud;
        public IListHandlerSubview Scroll => defaultSubviewTable.Scroll;
        public IListHandlerSubview Widgets => defaultSubviewTable.Widgets;
        public IMessageBoxSubview LongMessage => defaultSubviewTable.LongMessage;
        public IListHandlerSubview BackAnchor => defaultSubviewTable.BackAnchor;
        public IListHandlerSubview ForwardAnchor => defaultSubviewTable.ForwardAnchor;
        public IListHandlerSubview PrimaryCommand => defaultSubviewTable.PrimaryCommand;
        public IMessageBoxSubview CaptionBox => defaultSubviewTable.CaptionBox;
        public IListHandlerSubview SecondaryCommand => defaultSubviewTable.SecondaryCommand;
        public IListHandlerSubview Dialog => defaultSubviewTable.Dialog;
        public IColorPickerSubview ColorPicker => defaultSubviewTable.ColorPicker;
        public IMessageBoxSubview MessageBox => defaultSubviewTable.MessageBox;
        public IListHandlerSubview FadeMask => defaultSubviewTable.FadeMask;
        public IListHandlerSubview Overlay => defaultSubviewTable.Overlay;
        public IMessageBoxSubview SpeechBox => defaultSubviewTable.SpeechBox;
        public IListHandlerSubview Choices => defaultSubviewTable.Choices;
        public IListHandlerSubview DropdownList => defaultSubviewTable.DropdownList;
        public IListHandlerSubview DropdownGrid => defaultSubviewTable.DropdownGrid;

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
            if (reservedScreen == null || HasManagerLock) return;

            try
            {
                reservedScreen.OpenScreen((TMgr)this);
                reservedScreen = null;
            }
            catch
            {
                reservedScreen = null;
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

        public void SetInvisibleDropdownPosition(Rect rect)
        {
            defaultSubviewTable.SetInvisibleDropdownPosition(rect);
        }

        public virtual string Localize(string text) => text?.Normalize(NormalizationForm.FormC); // TextMeshPro のために NFD を NFC に正規化する

        // 設計メモ: Stack<T>.Peek にならってプロパティではなくメソッドにする
        public IListuiScreen PeekScreenOrDefault() => stack.TryPeek(out var item) ? item.Screen : null;

        /// <summary>
        /// メニューを指定の画面へ進める
        /// </summary>
        public virtual void PushScreen(IListuiScreen<TMgr> screen)
        {
            screen.CloseScreenView((TMgr)this, false);
            BlockAll();
            reservedScreen = stack.Push(screen);
        }

        public void PushScreen<TArg>(IListuiScreen<TMgr, TArg> screen, TArg arg)
        {
            screen.CloseScreenView((TMgr)this, false, arg);
            BlockAll();
            reservedScreen = stack.Push(screen, arg);
        }

        public void PushInitialScreen(IListuiScreen<TMgr> screen, bool enableTouchMask = true)
        {
            stack.Clear();
            PushScreen(screen);
            defaultSubviewTable.SetBlocker(enableTouchMask);
        }

        public void PushInitialScreen<TArg>(IListuiScreen<TMgr, TArg> screen, TArg arg, bool enableTouchMask = true)
        {
            stack.Clear();
            PushScreen(screen, arg);
            defaultSubviewTable.SetBlocker(enableTouchMask);
        }

        /// <summary>
        /// メニュー画面を指定の回数戻る
        /// </summary>
        public void PopScreen(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (stack.Count == 0) break;

                var item = stack.Pop();
                item.CloseScreenView((TMgr)this, true);
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
                reservedScreen = stackItem;
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
            reservedScreen = null;
            HideAll();
            defaultSubviewTable.SetBlocker(false);
        }
    }
}
