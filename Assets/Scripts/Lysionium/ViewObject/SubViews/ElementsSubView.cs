using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// モデルのリストをコントローラで制御する UI のクラス。
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ElementsSubView : ElementsSubViewBase, IElementsSubView
    {
        private CanvasGroup canvasGroup;

        protected event HandleEndAnimation OnEndAnimation;

        /// <summary>
        /// この SubView 内で最後に選択された <see cref="GameObject"/>
        /// </summary>
        protected GameObject LastSelectedObj { get; private set; }
        protected ViewElement LastSelectedViewElement { get; private set; }

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

        private const int backStatusCode = 1;

        public abstract void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider);

        public void SetStatusCode(int statusCode)
        {
            AnimatorTupple.TrySetStatusCode(this, statusCode);
        }

        /// <summary>
        /// この SubView のUI操作をブロックしてプレイヤーからの操作を防ぐ（使用例: ダイアログの後ろで表示されているメニューをブロックする）
        /// </summary>
        public virtual void SetInteractable(bool interactable)
        {
            if (canvasGroup == null) { canvasGroup = GetComponent<CanvasGroup>(); }

            canvasGroup.interactable = interactable;
        }

        public virtual void Show(HandleEndAnimation onEndAnimation = null)
        {
            SetInteractable(true);
            AnimatorTupple.TrySetVisible(this, true);

            if (onEndAnimation != null)
            {
                OnEndAnimation += onEndAnimation;
            }
        }

        public virtual void Hide(bool back, HandleEndAnimation onEndAnimation = null)
        {
            if (back) { SetStatusCode(backStatusCode); }
            SetInteractable(false);
            AnimatorTupple.TrySetVisible(this, false);

            if (onEndAnimation != null)
            {
                OnEndAnimation += onEndAnimation;
            }
        }

        public override void OnSelectViewElement(GameObject selectedObj, bool outOfRange)
        {
            if (!outOfRange)
            {
                LastSelectedObj = selectedObj;
                LastSelectedViewElement = selectedObj.GetComponent<ViewElement>();
            }
            AnimatorTupple.OnSelect(this, selectedObj, outOfRange);
        }

        /// <summary>
        /// カーソルを移動させる。使用時は一番上に表示されているメニューのカーソル移動（特に初期選択）を阻害しないように気を付ける
        /// </summary>
        public override void QueueSelect(GameObject sender, GameObject to, CursorPlay play)
        {
            LastSelectedObj = to;
            LastSelectedViewElement = to != null ? to.GetComponent<ViewElement>() : null;
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
