using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Lysionium.Views
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
        private KeybindTuple binding;

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
        public void Keybind(
            System.ReadOnlySpan<char> style,
            System.Action<InputAction.CallbackContext> performed = null,
            System.Action<InputAction.CallbackContext> started = null,
            System.Action<InputAction.CallbackContext> canceled = null)
            => KeybindTuple.Keybind(this, style, performed, started, canceled);
        public void Keyunbind(
            System.ReadOnlySpan<char> style,
            System.Action<InputAction.CallbackContext> performed = null,
            System.Action<InputAction.CallbackContext> started = null,
            System.Action<InputAction.CallbackContext> canceled = null)
            => KeybindTuple.Keyunbind(this, style, performed, started, canceled);



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

            public static void TrySetBack(SubviewBase subview, bool back)
            {
                var animator = subview.animator ??= new AnimatorTupple(subview);
                if (!animator.IsEnabled) return;

                var parameterName = animator.subviewAnimator.BackBool;
                animator.animator.SetBool(parameterName, back);
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

        internal class KeybindTuple
        {
            private readonly KeybindStyleSheet keybindStyleSheet;

            private bool IsEnabled => keybindStyleSheet != null;

            public KeybindTuple(SubviewBase subview)
            {
                keybindStyleSheet = subview.GetComponentInParent<KeybindStyleSheet>();
            }

            public static void Keybind(
                SubviewBase subview, System.ReadOnlySpan<char> style,
                System.Action<InputAction.CallbackContext> performed,
                System.Action<InputAction.CallbackContext> started,
                System.Action<InputAction.CallbackContext> canceled)
            {
                var binding = subview.binding ??= new KeybindTuple(subview);
                if (!binding.IsEnabled) return;

                binding.keybindStyleSheet.Keybind(style, performed, started, canceled);
            }

            public static void Keyunbind(
                SubviewBase subview, System.ReadOnlySpan<char> style,
                System.Action<InputAction.CallbackContext> performed,
                System.Action<InputAction.CallbackContext> started,
                System.Action<InputAction.CallbackContext> canceled)
            {
                var binding = subview.binding ??= new KeybindTuple(subview);
                if (!binding.IsEnabled) return;

                binding.keybindStyleSheet.Keyunbind(style, performed, started, canceled);
            }
        }
    }
}
