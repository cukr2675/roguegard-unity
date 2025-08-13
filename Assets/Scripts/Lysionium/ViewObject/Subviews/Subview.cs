using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// モデルのリストをコントローラで制御する UI のクラス。
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Subview : SubviewBase, ISubview
    {
        private CanvasGroup canvasGroup;

        protected event ListMenuEventHandler OnEndAnimation;
        protected event ListMenuEventHandler OnHide;

        /// <summary>
        /// この Subview 内で最後に選択された <see cref="GameObject"/>
        /// </summary>
        protected GameObject LastSelectedObject { get; private set; }
        protected ViewItem LastSelectedItem { get; private set; }

        /// <summary>
        /// このプロパティが true のとき <see cref="IListMenuManager"/> の動作を停止させる。アニメーションを待機させるために使用する
        /// </summary>
        public bool HasManagerLock { get; private set; }

        public bool Interactable
        {
            get
            {
                if (canvasGroup == null) { canvasGroup = GetComponent<CanvasGroup>(); }

                return canvasGroup.interactable;
            }
        }

        private bool isInitialized;

        private const int backStatusCode = 1;

        public void CommonInit()
        {
            LuiAssert.NotInitialized(this, isInitialized);
            isInitialized = true;
            CommonInitCore();
        }

        /// <summary>
        /// <see cref="CommonInit"/> 内で呼び出すメソッド。
        /// 言語変更などで <see cref="IListMenuManager.Localize"/> が変わる可能性があるため、
        /// このメソッド内で <see cref="ViewItem.Bind"/> を呼び出してはならない
        /// </summary>
        protected virtual void CommonInitCore() { }

        protected virtual void OnDestroy()
        {
            var tempOnHide = OnHide;
            OnHide = null;
            tempOnHide?.Invoke(Manager, Arg);
        }

        public abstract void SetParameters(
            IReadOnlyList<object> list, IViewItemHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref ISubviewStateProvider stateProvider);

        public void SetStatusCode(int statusCode)
        {
            AnimatorTupple.TrySetStatusCode(this, statusCode);
        }

        /// <summary>
        /// この Subview のUI操作をブロックしてプレイヤーからの操作を防ぐ（使用例: ダイアログの後ろで表示されているメニューをブロックする）
        /// </summary>
        public virtual void SetInteractable(bool interactable)
        {
            if (canvasGroup == null) { canvasGroup = GetComponent<CanvasGroup>(); }

            canvasGroup.interactable = interactable;
        }

        public virtual void Show(ListMenuEventHandler onEndAnimation = null, ListMenuEventHandler onHide = null)
        {
            var tempOnHide = OnHide;
            OnHide = null;
            tempOnHide?.Invoke(Manager, Arg);

            SetInteractable(true);
            AnimatorTupple.TrySetVisible(this, true);

            if (onEndAnimation != null) { OnEndAnimation += onEndAnimation; }
            if (onHide != null) { OnHide += onHide; }
        }

        public virtual void Hide(bool back, ListMenuEventHandler onEndAnimation = null)
        {
            var tempOnHide = OnHide;
            OnHide = null;
            tempOnHide?.Invoke(Manager, Arg);

            if (back) { SetStatusCode(backStatusCode); }
            SetInteractable(false);
            AnimatorTupple.TrySetVisible(this, false);

            if (onEndAnimation != null) { OnEndAnimation += onEndAnimation; }
        }

        public override void OnSelectItem(GameObject selectedObj, bool outOfRange)
        {
            if (!outOfRange)
            {
                LastSelectedObject = selectedObj;
                LastSelectedItem = selectedObj.GetComponent<ViewItem>();
            }
            AnimatorTupple.OnSelect(this, selectedObj, outOfRange);
        }

        /// <summary>
        /// カーソルを移動させる。使用時は一番上に表示されているメニューのカーソル移動（特に初期選択）を阻害しないように気を付ける
        /// </summary>
        public override void QueueSelect(GameObject sender, GameObject to, CursorPlay play)
        {
            LastSelectedObject = to;
            LastSelectedItem = to != null ? to.GetComponent<ViewItem>() : null;
            AnimatorTupple.QueueSelect(this, sender, to, play);
        }

        public override void QueueSelectToLastSelectedObj(GameObject sender, CursorPlay play)
        {
            AnimatorTupple.QueueSelectToLastSelectedObj(this, sender, play);
        }

        // Animation から呼び出すメソッド
        public void LockManager() => HasManagerLock = true;
        public void UnlockManager()
        {
            HasManagerLock = false;
            var tempOnEndAnimation = OnEndAnimation;
            OnEndAnimation = null;
            tempOnEndAnimation?.Invoke(Manager, Arg);
        }
        public void PlayString(string value) => AnimatorTupple.Play(this, this, value);
        public void PlayObject(Object value) => AnimatorTupple.Play(this, this, value);
    }
}
