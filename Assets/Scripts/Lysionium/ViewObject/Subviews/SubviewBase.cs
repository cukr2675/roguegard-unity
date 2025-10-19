using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Lysionium
{
    /// <summary>
    /// モデルのリストをコントローラで制御する UI のクラス。
    /// </summary>
    public abstract class SubviewBase : MonoBehaviour
    {
        private EventSystem _eventSystem;
        public EventSystem EventSystem => _eventSystem ? _eventSystem : _eventSystem = LuiUtility.GetEventSystem(this);

        public IListMenuManager Manager { get; private set; }
        public IListMenuArg Arg { get; private set; }

        private AnimatorTupple animator;
        private KeyBindTuple binding;

        protected void SetArg(IListMenuManager manager, IListMenuArg arg)
        {
            Manager = manager;
            Arg = arg;
        }

        public abstract void OnSelectItem(GameObject selectedObj, bool outOfRange);
        public abstract void QueueSelect(GameObject sender, GameObject to, CursorPlay play);
        public abstract void QueueSelectToLastSelectedObj(GameObject sender, CursorPlay play);

        // ViewItem から呼び出すメソッド
        public void PlayFromItem(string value, Object item) => AnimatorTupple.Play(this, item, value);
        public void PlayFromItem(Object value, Object item) => AnimatorTupple.Play(this, item, value);
        public bool TryGetKeyIcon(System.ReadOnlySpan<char> style, out string keyText, out Sprite keySprite)
            => KeyBindTuple.TryGetKeyIcon(this, style, out keyText, out keySprite);
        public void KeyBind(
            System.ReadOnlySpan<char> style,
            System.Action<InputAction.CallbackContext> performed = null,
            System.Action<InputAction.CallbackContext> started = null,
            System.Action<InputAction.CallbackContext> canceled = null)
            => KeyBindTuple.KeyBind(this, style, performed, started, canceled);
        public void Unbind(
            System.ReadOnlySpan<char> style,
            System.Action<InputAction.CallbackContext> performed = null,
            System.Action<InputAction.CallbackContext> started = null,
            System.Action<InputAction.CallbackContext> canceled = null)
            => KeyBindTuple.Unbind(this, style, performed, started, canceled);



        internal class AnimatorTupple
        {
            private readonly Animator animator;
            private readonly SubviewAnimator subviewAnimator;

            private bool IsEnabled => animator != null && subviewAnimator != null;

            public AnimatorTupple(SubviewBase subview)
            {
                subview.TryGetComponent(out animator);
                subviewAnimator = subview.GetComponentInParent<SubviewAnimator>();
            }

            public static void TrySetVisible(SubviewBase subview, bool visible)
            {
                var animator = subview.animator ??= new AnimatorTupple(subview);
                if (!animator.IsEnabled) return;

                var parameterName = animator.subviewAnimator.VisibleBool;
                animator.animator.SetBool(parameterName, visible);
            }

            public static void TrySetStatusCode(SubviewBase subview, int statusCode)
            {
                var animator = subview.animator ??= new AnimatorTupple(subview);
                if (!animator.IsEnabled) return;

                var parameterName = animator.subviewAnimator.StatusCodeInteger;
                animator.animator.SetInteger(parameterName, statusCode);
            }

            public static void Play(SubviewBase subview, Object sender, string value)
            {
                var animator = subview.animator ??= new AnimatorTupple(subview);
                if (!animator.IsEnabled) return;

                animator.subviewAnimator.OnPlayString.Invoke(value, sender);
            }

            public static void Play(SubviewBase subview, Object sender, Object value)
            {
                var animator = subview.animator ??= new AnimatorTupple(subview);
                if (!animator.IsEnabled) return;

                animator.subviewAnimator.OnPlayObject.Invoke(value, sender);
            }

            public static void OnSelect(SubviewBase subview, GameObject gameObject, bool outOfRange)
            {
                var animator = subview.animator ??= new AnimatorTupple(subview);
                if (!animator.IsEnabled) return;

                animator.subviewAnimator.OnSelect(gameObject, outOfRange);
            }

            public static void QueueSelect(SubviewBase subview, GameObject sender, GameObject to, CursorPlay play)
            {
                var animator = subview.animator ??= new AnimatorTupple(subview);
                if (!animator.IsEnabled) return;

                animator.subviewAnimator.QueueSelect(sender, to, play);
            }

            public static void QueueSelectToLastSelectedObj(SubviewBase subview, GameObject sender, CursorPlay play)
            {
                var animator = subview.animator ??= new AnimatorTupple(subview);
                if (!animator.IsEnabled) return;

                animator.subviewAnimator.QueueSelectToLastSelectedObj(sender, play);
            }
        }

        internal class KeyBindTuple
        {
            private readonly KeyBindStyleSheet keyBindStyleSheet;

            private bool IsEnabled => keyBindStyleSheet != null;

            public KeyBindTuple(SubviewBase subview)
            {
                keyBindStyleSheet = subview.GetComponentInParent<KeyBindStyleSheet>();
            }

            public static bool TryGetKeyIcon(
                SubviewBase subview, System.ReadOnlySpan<char> style, out string keyText, out Sprite keySprite)
            {
                var binding = subview.binding ??= new KeyBindTuple(subview);
                if (!binding.IsEnabled)
                {
                    keyText = null;
                    keySprite = null;
                    return false;
                }

                return binding.keyBindStyleSheet.TryGetKeyIcon(style, out keyText, out keySprite);
            }

            public static void KeyBind(
                SubviewBase subview, System.ReadOnlySpan<char> style,
                System.Action<InputAction.CallbackContext> performed,
                System.Action<InputAction.CallbackContext> started,
                System.Action<InputAction.CallbackContext> canceled)
            {
                var binding = subview.binding ??= new KeyBindTuple(subview);
                if (!binding.IsEnabled) return;

                binding.keyBindStyleSheet.KeyBind(style, performed, started, canceled);
            }

            public static void Unbind(
                SubviewBase subview, System.ReadOnlySpan<char> style,
                System.Action<InputAction.CallbackContext> performed,
                System.Action<InputAction.CallbackContext> started,
                System.Action<InputAction.CallbackContext> canceled)
            {
                var binding = subview.binding ??= new KeyBindTuple(subview);
                if (!binding.IsEnabled) return;

                binding.keyBindStyleSheet.Unbind(style, performed, started, canceled);
            }
        }
    }
}
