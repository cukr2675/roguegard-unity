using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

namespace ListingMF
{
    /// <summary>
    /// モデルのリストをコントローラで制御する UI のクラス。
    /// </summary>
    public abstract class ElementsSubView : MonoBehaviour, IElementsSubView
    {
        public IListMenuManager Manager { get; private set; }
        public IListMenuArg Arg { get; private set; }

        private event HandleEndAnimation OnEndAnimation;
        private AnimatorTupple animator;

        protected ViewElement LastSelectedViewElement { get; private set; }

        /// <summary>
        /// <see cref="SetBlock"/> で <see cref="ViewElement.SetBlock"/> を呼び出す対象
        /// </summary>
        protected virtual IReadOnlyList<ViewElement> BlockableViewElements => System.Array.Empty<ViewElement>();

        public bool IsBlocked { get; protected set; }

        public virtual bool HasManagerLock { get; private set; }

        public delegate void HandleEndAnimation(IListMenuManager manager, IListMenuArg arg);

        private const int backStatusCode = 2;

        public abstract void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider);

        protected void SetArg(IListMenuManager manager, IListMenuArg arg)
        {
            Manager = manager;
            Arg = arg;
        }

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

        public void OnSelectViewElement(ViewElement viewElement, bool outOfRange)
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
        public void PlayString(string value) => AnimatorTupple.Play(this, value);
        public void PlayObject(Object value) => AnimatorTupple.Play(this, value);



        private class AnimatorTupple
        {
            private readonly Animator animator;
            private readonly ElementsViewAnimator viewAnimator;

            private bool IsEnabled => animator != null && viewAnimator != null;

            public AnimatorTupple(ElementsSubView elementsSubView)
            {
                elementsSubView.TryGetComponent(out animator);
                viewAnimator = ElementsViewAnimator.Get(elementsSubView);
            }

            public static void TrySetVisible(ElementsSubView elementsSubView, bool visible)
            {
                if (elementsSubView.animator == null) { elementsSubView.animator = new AnimatorTupple(elementsSubView); }

                var animator = elementsSubView.animator;
                if (!animator.IsEnabled) return;

                var parameterName = animator.viewAnimator.VisibleBool;
                animator.animator.SetBool(parameterName, visible);
            }

            public static void TrySetStatusCode(ElementsSubView elementsSubView, int statusCode)
            {
                if (elementsSubView.animator == null) { elementsSubView.animator = new AnimatorTupple(elementsSubView); }

                var animator = elementsSubView.animator;
                if (!animator.IsEnabled) return;

                var parameterName = animator.viewAnimator.StatusCodeInteger;
                animator.animator.SetInteger(parameterName, statusCode);
            }

            public static void Play(ElementsSubView elementsSubView, string value)
            {
                if (elementsSubView.animator == null) { elementsSubView.animator = new AnimatorTupple(elementsSubView); }

                var animator = elementsSubView.animator;
                if (!animator.IsEnabled) return;

                animator.viewAnimator.OnPlayString.Invoke(value);
            }

            public static void Play(ElementsSubView elementsSubView, Object value)
            {
                if (elementsSubView.animator == null) { elementsSubView.animator = new AnimatorTupple(elementsSubView); }

                var animator = elementsSubView.animator;
                if (!animator.IsEnabled) return;

                animator.viewAnimator.OnPlayObject.Invoke(value);
            }

            public static void OnSelect(ElementsSubView elementsSubView, GameObject gameObject, bool outOfRange)
            {
                if (elementsSubView.animator == null) { elementsSubView.animator = new AnimatorTupple(elementsSubView); }

                var animator = elementsSubView.animator;
                if (!animator.IsEnabled) return;

                animator.viewAnimator.OnSelect(gameObject, outOfRange);
            }
        }
    }
}
