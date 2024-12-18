using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

namespace ListingMF
{
    /// <summary>
    /// モデルのリストをコントローラで制御する UI のクラス。
    /// </summary>
    public abstract class ElementsSubView : ElementsSubViewBase, IElementsSubView
    {
        private event HandleEndAnimation OnEndAnimation;

        protected ViewElement LastSelectedViewElement { get; private set; }

        /// <summary>
        /// <see cref="SetBlock"/> で <see cref="ViewElement.SetBlock"/> を呼び出す対象
        /// </summary>
        protected virtual IReadOnlyList<ViewElement> BlockableViewElements => System.Array.Empty<ViewElement>();

        public virtual bool HasManagerLock { get; private set; }

        public delegate void HandleEndAnimation(IListMenuManager manager, IListMenuArg arg);

        private const int backStatusCode = 2;

        public abstract void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider);

        public void SetStatusCode(int statusCode)
        {
            AnimatorTupple.TrySetStatusCode(this, statusCode);
        }

        public virtual void SetBlock(bool block)
        {
            IsBlocked = block;
            for (int i = 0; i < BlockableViewElements.Count; i++)
            {
                BlockableViewElements[i].SetBlock(block);
            }
        }

        public virtual void Show(HandleEndAnimation handleEndAnimation = null)
        {
            SetBlock(false);
            AnimatorTupple.TrySetVisible(this, true);

            if (handleEndAnimation != null)
            {
                OnEndAnimation += handleEndAnimation;
            }
        }

        public virtual void Hide(bool back, HandleEndAnimation handleEndAnimation = null)
        {
            if (back) { SetStatusCode(backStatusCode); }
            SetBlock(true);
            AnimatorTupple.TrySetVisible(this, false);

            if (handleEndAnimation != null)
            {
                OnEndAnimation += handleEndAnimation;
            }
        }

        public override void OnSelectViewElement(ViewElement viewElement, bool outOfRange)
        {
            LastSelectedViewElement = viewElement;
            AnimatorTupple.OnSelect(this, viewElement.gameObject, outOfRange);
        }

        // Animation から呼び出すメソッド
        public void LockManager() => HasManagerLock = true;
        public void UnlockManager()
        {
            HasManagerLock = false;
            OnEndAnimation?.Invoke(Manager, Arg);
            OnEndAnimation = null;
        }
        public void PlayString(string value) => AnimatorTupple.Play(this, this, value);
        public void PlayObject(Object value) => AnimatorTupple.Play(this, this, value);
    }
}
