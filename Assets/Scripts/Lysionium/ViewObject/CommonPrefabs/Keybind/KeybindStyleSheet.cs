using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lysionium.Views
{
    // 命名メモ: スタイル名で指定するので StyleSheet
    /// <summary>
    /// 要素のスタイルでキーバインドするスタイルシート。このオブジェクトの下の <see cref="Subview"/> に影響を与える
    /// </summary>
    [AddComponentMenu("UI/Lysionium/LUI Keybind Style Sheet")]
    public class KeybindStyleSheet : MonoBehaviour
    {
        [SerializeField] private KeybindGlyphAsset _keybindGlyphAsset;
        public KeybindGlyphAsset KeybindGlyphAsset
        {
            get => _keybindGlyphAsset;
            set => _keybindGlyphAsset = value;
        }

        [SerializeField] private Binding[] _bindings = null;
        public IReadOnlyList<Binding> Bindings => _bindings;

        private PlayerInput playerInput;

        protected virtual void Start()
        {
            if (_keybindGlyphAsset != null)
            {
                _keybindGlyphAsset.UpdateGlyphs(_bindings);

                // 影響があると思われるテキストオブジェクトを更新する
                var texts = GetComponentsInChildren<TMP_Text>();
                foreach (var text in texts)
                {
                    text.SetAllDirty();
                }
            }
        }

        public bool TryGetAction(ReadOnlySpan<char> style, out InputAction action)
        {
            foreach (var binding in _bindings)
            {
                if (!binding.Style.AsSpan().SequenceEqual(style)) continue;

                action = binding.Action;
                return true;
            }
            action = null;
            return false;
        }

        public void Keybind(
            ReadOnlySpan<char> style,
            Action<InputAction.CallbackContext> performed,
            Action<InputAction.CallbackContext> started,
            Action<InputAction.CallbackContext> canceled)
        {
            if (!TryGetAction(style, out var action)) return;

            if (playerInput == null) { playerInput = GetComponentInParent<PlayerInput>(); }
            if (playerInput != null) { action = playerInput.actions[action.name]; }
            else { action.Enable(); }
            if (performed != null) { action.performed += performed; }
            if (started != null) { action.started += started; }
            if (canceled != null) { action.canceled += canceled; }
        }

        public void Keyunbind(
            ReadOnlySpan<char> style,
            Action<InputAction.CallbackContext> performed,
            Action<InputAction.CallbackContext> started,
            Action<InputAction.CallbackContext> canceled)
        {
            if (!TryGetAction(style, out var action)) return;

            if (playerInput == null) { playerInput = GetComponentInParent<PlayerInput>(); }
            if (playerInput != null) { action = playerInput.actions[action.name]; }
            //else { action.Disable(); } // バインディングされているアクションが一つとは限らないため無効化しない
            if (performed != null) { action.performed -= performed; }
            if (started != null) { action.started -= started; }
            if (canceled != null) { action.canceled -= canceled; }
        }

        protected virtual void OnDestroy()
        {
            foreach (var binding in _bindings)
            {
                binding.Action?.Disable();
            }
        }

        [Serializable]
        public class Binding
        {
            //[SerializeField] private string _style;
            public string Style => _action.action.name;

            [SerializeField] private InputActionReference _action;
            public InputAction Action => _action;
        }
    }
}
