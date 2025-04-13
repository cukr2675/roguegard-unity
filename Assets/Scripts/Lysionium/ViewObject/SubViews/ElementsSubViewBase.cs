using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

namespace Lysionium
{
    /// <summary>
    /// モデルのリストをコントローラで制御する UI のクラス。
    /// </summary>
    public abstract class ElementsSubViewBase : MonoBehaviour
    {
        public IListMenuManager Manager { get; private set; }
        public IListMenuArg Arg { get; private set; }

        private AnimatorTupple animator;
        private KeyBindTuple binding;

        protected void SetArg(IListMenuManager manager, IListMenuArg arg)
        {
            Manager = manager;
            Arg = arg;
        }

        public abstract void OnSelectViewElement(GameObject selectedObj, bool outOfRange);
        public abstract void QueueSelect(GameObject sender, GameObject to, CursorPlay play);
        public abstract void QueueSelectToLastSelectedObj(GameObject sender, CursorPlay play);

        // ViewElement から呼び出すメソッド
        public void PlayFromElement(string value, Object element) => AnimatorTupple.Play(this, element, value);
        public void PlayFromElement(Object value, Object element) => AnimatorTupple.Play(this, element, value);
        public bool TryGetKeyIcon(System.ReadOnlySpan<char> style, out string keyText, out Sprite keySprite)
            => KeyBindTuple.TryGetKeyIcon(this, style, out keyText, out keySprite);
        public void KeyBind(System.ReadOnlySpan<char> style, System.Action<InputAction.CallbackContext> performed)
            => KeyBindTuple.KeyBind(this, style, performed);
        public void Unbind(System.ReadOnlySpan<char> style, System.Action<InputAction.CallbackContext> performed)
            => KeyBindTuple.Unbind(this, style, performed);



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

            public static void QueueSelect(ElementsSubViewBase elementsSubView, GameObject sender, GameObject to, CursorPlay play)
            {
                var animator = elementsSubView.animator ??= new AnimatorTupple(elementsSubView);
                if (!animator.IsEnabled) return;

                animator.viewAnimator.QueueSelect(sender, to, play);
            }

            public static void QueueSelectToLastSelectedObj(ElementsSubViewBase elementsSubView, GameObject sender, CursorPlay play)
            {
                var animator = elementsSubView.animator ??= new AnimatorTupple(elementsSubView);
                if (!animator.IsEnabled) return;

                animator.viewAnimator.QueueSelectToLastSelectedObj(sender, play);
            }
        }

        internal class KeyBindTuple
        {
            private readonly KeyBindStyleSheet keyBindStyleSheet;

            private bool IsEnabled => keyBindStyleSheet != null;

            public KeyBindTuple(ElementsSubViewBase elementsSubView)
            {
                keyBindStyleSheet = KeyBindStyleSheet.Get(elementsSubView);
            }

            public static bool TryGetKeyIcon(
                ElementsSubViewBase elementsSubView, System.ReadOnlySpan<char> style, out string keyText, out Sprite keySprite)
            {
                var binding = elementsSubView.binding ??= new KeyBindTuple(elementsSubView);
                if (!binding.IsEnabled)
                {
                    keyText = null;
                    keySprite = null;
                    return false;
                }

                return binding.keyBindStyleSheet.TryGetKeyIcon(style, out keyText, out keySprite);
            }

            public static void KeyBind(
                ElementsSubViewBase elementsSubView, System.ReadOnlySpan<char> style, System.Action<InputAction.CallbackContext> performed)
            {
                var binding = elementsSubView.binding ??= new KeyBindTuple(elementsSubView);
                if (!binding.IsEnabled) return;

                binding.keyBindStyleSheet.KeyBind(style, performed);
            }

            public static void Unbind(
                ElementsSubViewBase elementsSubView, System.ReadOnlySpan<char> style, System.Action<InputAction.CallbackContext> performed)
            {
                var binding = elementsSubView.binding ??= new KeyBindTuple(elementsSubView);
                if (!binding.IsEnabled) return;

                binding.keyBindStyleSheet.Unbind(style, performed);
            }
        }
    }
}
