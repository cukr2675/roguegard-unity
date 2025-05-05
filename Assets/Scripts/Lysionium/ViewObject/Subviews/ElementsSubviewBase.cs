using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

namespace Lysionium
{
    /// <summary>
    /// モデルのリストをコントローラで制御する UI のクラス。
    /// </summary>
    public abstract class ElementsSubviewBase : MonoBehaviour
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

            public AnimatorTupple(ElementsSubviewBase elementsSubview)
            {
                elementsSubview.TryGetComponent(out animator);
                viewAnimator = ElementsViewAnimator.Get(elementsSubview);
            }

            public static void TrySetVisible(ElementsSubviewBase elementsSubview, bool visible)
            {
                var animator = elementsSubview.animator ??= new AnimatorTupple(elementsSubview);
                if (!animator.IsEnabled) return;

                var parameterName = animator.viewAnimator.VisibleBool;
                animator.animator.SetBool(parameterName, visible);
            }

            public static void TrySetStatusCode(ElementsSubviewBase elementsSubview, int statusCode)
            {
                var animator = elementsSubview.animator ??= new AnimatorTupple(elementsSubview);
                if (!animator.IsEnabled) return;

                var parameterName = animator.viewAnimator.StatusCodeInteger;
                animator.animator.SetInteger(parameterName, statusCode);
            }

            public static void Play(ElementsSubviewBase elementsSubview, Object sender, string value)
            {
                var animator = elementsSubview.animator ??= new AnimatorTupple(elementsSubview);
                if (!animator.IsEnabled) return;

                animator.viewAnimator.OnPlayString.Invoke(value, sender);
            }

            public static void Play(ElementsSubviewBase elementsSubview, Object sender, Object value)
            {
                var animator = elementsSubview.animator ??= new AnimatorTupple(elementsSubview);
                if (!animator.IsEnabled) return;

                animator.viewAnimator.OnPlayObject.Invoke(value, sender);
            }

            public static void OnSelect(ElementsSubviewBase elementsSubview, GameObject gameObject, bool outOfRange)
            {
                var animator = elementsSubview.animator ??= new AnimatorTupple(elementsSubview);
                if (!animator.IsEnabled) return;

                animator.viewAnimator.OnSelect(gameObject, outOfRange);
            }

            public static void QueueSelect(ElementsSubviewBase elementsSubview, GameObject sender, GameObject to, CursorPlay play)
            {
                var animator = elementsSubview.animator ??= new AnimatorTupple(elementsSubview);
                if (!animator.IsEnabled) return;

                animator.viewAnimator.QueueSelect(sender, to, play);
            }

            public static void QueueSelectToLastSelectedObj(ElementsSubviewBase elementsSubview, GameObject sender, CursorPlay play)
            {
                var animator = elementsSubview.animator ??= new AnimatorTupple(elementsSubview);
                if (!animator.IsEnabled) return;

                animator.viewAnimator.QueueSelectToLastSelectedObj(sender, play);
            }
        }

        internal class KeyBindTuple
        {
            private readonly KeyBindStyleSheet keyBindStyleSheet;

            private bool IsEnabled => keyBindStyleSheet != null;

            public KeyBindTuple(ElementsSubviewBase elementsSubview)
            {
                keyBindStyleSheet = KeyBindStyleSheet.Get(elementsSubview);
            }

            public static bool TryGetKeyIcon(
                ElementsSubviewBase elementsSubview, System.ReadOnlySpan<char> style, out string keyText, out Sprite keySprite)
            {
                var binding = elementsSubview.binding ??= new KeyBindTuple(elementsSubview);
                if (!binding.IsEnabled)
                {
                    keyText = null;
                    keySprite = null;
                    return false;
                }

                return binding.keyBindStyleSheet.TryGetKeyIcon(style, out keyText, out keySprite);
            }

            public static void KeyBind(
                ElementsSubviewBase elementsSubview, System.ReadOnlySpan<char> style, System.Action<InputAction.CallbackContext> performed)
            {
                var binding = elementsSubview.binding ??= new KeyBindTuple(elementsSubview);
                if (!binding.IsEnabled) return;

                binding.keyBindStyleSheet.KeyBind(style, performed);
            }

            public static void Unbind(
                ElementsSubviewBase elementsSubview, System.ReadOnlySpan<char> style, System.Action<InputAction.CallbackContext> performed)
            {
                var binding = elementsSubview.binding ??= new KeyBindTuple(elementsSubview);
                if (!binding.IsEnabled) return;

                binding.keyBindStyleSheet.Unbind(style, performed);
            }
        }
    }
}
