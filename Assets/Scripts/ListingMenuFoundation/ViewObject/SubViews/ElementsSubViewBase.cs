using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

namespace ListingMF
{
    /// <summary>
    /// モデルのリストをコントローラで制御する UI のクラス。
    /// </summary>
    public abstract class ElementsSubViewBase : MonoBehaviour
    {
        public IListMenuManager Manager { get; private set; }
        public IListMenuArg Arg { get; private set; }

        public bool IsBlocked { get; protected set; }

        private AnimatorTupple animator;
        private BindingTuple binding;

        protected void SetArg(IListMenuManager manager, IListMenuArg arg)
        {
            Manager = manager;
            Arg = arg;
        }

        public abstract void OnSelectViewElement(ViewElement viewElement, bool outOfRange);

        // ViewElement から呼び出すメソッド
        public void PlayFromElement(string value, Object element) => AnimatorTupple.Play(this, element, value);
        public void PlayFromElement(Object value, Object element) => AnimatorTupple.Play(this, element, value);
        public void Bind(System.ReadOnlySpan<char> style, System.Action<InputAction.CallbackContext> performed)
            => BindingTuple.Bind(this, style, performed);
        public void Unbind(System.ReadOnlySpan<char> style, System.Action<InputAction.CallbackContext> performed)
            => BindingTuple.Unbind(this, style, performed);



        internal class AnimatorTupple
        {
            private readonly Animator animator;
            private readonly ElementsViewAnimator viewAnimator;

            private bool IsEnabled => animator != null && viewAnimator != null;

            public AnimatorTupple(ElementsSubViewBase elementsSubView)
            {
                elementsSubView.TryGetComponent(out animator);
                viewAnimator = ElementsViewAnimator.Get(elementsSubView);
            }

            public static void TrySetVisible(ElementsSubViewBase elementsSubView, bool visible)
            {
                var animator = elementsSubView.animator ??= new AnimatorTupple(elementsSubView);
                if (!animator.IsEnabled) return;

                var parameterName = animator.viewAnimator.VisibleBool;
                animator.animator.SetBool(parameterName, visible);
            }

            public static void TrySetStatusCode(ElementsSubViewBase elementsSubView, int statusCode)
            {
                var animator = elementsSubView.animator ??= new AnimatorTupple(elementsSubView);
                if (!animator.IsEnabled) return;

                var parameterName = animator.viewAnimator.StatusCodeInteger;
                animator.animator.SetInteger(parameterName, statusCode);
            }

            public static void Play(ElementsSubViewBase elementsSubView, Object sender, string value)
            {
                var animator = elementsSubView.animator ??= new AnimatorTupple(elementsSubView);
                if (!animator.IsEnabled) return;

                animator.viewAnimator.OnPlayString.Invoke(value, sender);
            }

            public static void Play(ElementsSubViewBase elementsSubView, Object sender, Object value)
            {
                var animator = elementsSubView.animator ??= new AnimatorTupple(elementsSubView);
                if (!animator.IsEnabled) return;

                animator.viewAnimator.OnPlayObject.Invoke(value, sender);
            }

            public static void OnSelect(ElementsSubViewBase elementsSubView, GameObject gameObject, bool outOfRange)
            {
                var animator = elementsSubView.animator ??= new AnimatorTupple(elementsSubView);
                if (!animator.IsEnabled) return;

                animator.viewAnimator.OnSelect(gameObject, outOfRange);
            }
        }

        internal class BindingTuple
        {
            private readonly InputSystemBindingStyleSheet inputSystemBindingStyleSheet;

            private bool IsEnabled => inputSystemBindingStyleSheet != null;

            public BindingTuple(ElementsSubViewBase elementsSubView)
            {
                inputSystemBindingStyleSheet = InputSystemBindingStyleSheet.Get(elementsSubView);
            }

            public static void Bind(
                ElementsSubViewBase elementsSubView, System.ReadOnlySpan<char> style, System.Action<InputAction.CallbackContext> performed)
            {
                var binding = elementsSubView.binding ??= new BindingTuple(elementsSubView);
                if (!binding.IsEnabled) return;

                binding.inputSystemBindingStyleSheet.Bind(style, performed);
            }

            public static void Unbind(
                ElementsSubViewBase elementsSubView, System.ReadOnlySpan<char> style, System.Action<InputAction.CallbackContext> performed)
            {
                var binding = elementsSubView.binding ??= new BindingTuple(elementsSubView);
                if (!binding.IsEnabled) return;

                binding.inputSystemBindingStyleSheet.Unbind(style, performed);
            }
        }
    }
}
