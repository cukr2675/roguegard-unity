using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

namespace Lysionium
{
    /// <summary>
    /// 要素のスタイルでキーバインドするスタイルシート。このオブジェクトの下の <see cref="Subview"/> に影響を与える
    /// </summary>
    [AddComponentMenu("UI/Lysionium/LUI Key Bind Style Sheet")]
    public class KeyBindStyleSheet : MonoBehaviour
    {
        [SerializeField] private Sprite _keyboardIconBackground = null;

        [SerializeField] private Binding[] _bindings = null;

        private PlayerInput playerInput;

        public static KeyBindStyleSheet Get(Component obj)
        {
            LuiUtility.TryGetComponentInRecursiveParents<KeyBindStyleSheet>(obj.transform, out var styleSheet);
            return styleSheet;
        }

        public bool TryGetAction(ReadOnlySpan<char> style, out InputAction action)
        {
            foreach (var binding in _bindings)
            {
                if (!binding.Style.SequenceEqual(style)) continue;

                action = binding.Action;
                return true;
            }
            action = null;
            return false;
        }

        public bool TryGetKeyIcon(ReadOnlySpan<char> style, out string keyText, out Sprite keySprite)
        {
            if (!TryGetAction(style, out var action))
            {
                keyText = null;
                keySprite = null;
                return false;
            }

            keyText = action.GetBindingDisplayString();
            keySprite = _keyboardIconBackground;
            return true;
        }

        public void KeyBind(ReadOnlySpan<char> style, Action<InputAction.CallbackContext> performed)
        {
            if (!TryGetAction(style, out var action)) return;

            if (playerInput == null) { LuiUtility.TryGetComponentInRecursiveParents(transform, out playerInput); }
            if (playerInput != null) { action = playerInput.actions[action.name]; }
            else { action.Enable(); }
            action.performed += performed;
        }

        public void Unbind(ReadOnlySpan<char> style, Action<InputAction.CallbackContext> performed)
        {
            if (!TryGetAction(style, out var action)) return;

            if (playerInput == null) { LuiUtility.TryGetComponentInRecursiveParents(transform, out playerInput); }
            if (playerInput != null) { action = playerInput.actions[action.name]; }
            //else { action.Disable(); } // バインディングされているアクションが一つとは限らないため無効化しない
            action.performed -= performed;
        }

        protected virtual void OnDestroy()
        {
            foreach (var binding in _bindings)
            {
                binding.Action?.Disable();
            }
        }

        [Serializable]
        private class Binding
        {
            //[SerializeField] private string _style;
            public ReadOnlySpan<char> Style => MemoryExtensions.AsSpan(_action.action.name);

            [SerializeField] public InputActionReference _action;
            public InputAction Action => _action;
        }
    }
}
