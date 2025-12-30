using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

        private EventSystem eventSystem;
        private PlayerInput playerInput;
        private bool keybindsAreEnabled;

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

        protected virtual void Update()
        {
            var newValue = GetKeybindIsEnabled();
            if (newValue != keybindsAreEnabled)
            {
                keybindsAreEnabled = newValue;
                foreach (var binding in _bindings)
                {
                    if (newValue) { binding.Action.Enable(); }
                    else { binding.Action.Disable(); }
                }
            }
        }

        private bool GetKeybindIsEnabled()
        {
            if (eventSystem == null)
            {
                eventSystem = LuiUtility.GetEventSystem(this);
            }
            if (eventSystem.currentSelectedGameObject == null ||
                !eventSystem.currentSelectedGameObject.TryGetComponent<TMP_InputField>(out var inputField)) return true;

            return !inputField.isFocused;
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
            Action<InputAction.CallbackContext> started,
            Action<InputAction.CallbackContext> performed,
            Action<InputAction.CallbackContext> canceled)
        {
            if (!TryGetAction(style, out var action)) return;

            if (playerInput == null) { playerInput = GetComponentInParent<PlayerInput>(); }
            if (playerInput != null) { action = playerInput.actions[action.name]; }
            else { action.Enable(); }
            if (started != null) { action.started += started; }
            if (performed != null) { action.performed += performed; }
            if (canceled != null) { action.canceled += canceled; }
        }

        public void Keyunbind(
            ReadOnlySpan<char> style,
            Action<InputAction.CallbackContext> started,
            Action<InputAction.CallbackContext> performed,
            Action<InputAction.CallbackContext> canceled)
        {
            if (!TryGetAction(style, out var action)) return;

            if (playerInput == null) { playerInput = GetComponentInParent<PlayerInput>(); }
            if (playerInput != null) { action = playerInput.actions[action.name]; }
            //else { action.Disable(); } // バインディングされているアクションが一つとは限らないため無効化しない
            if (started != null) { action.started -= started; }
            if (performed != null) { action.performed -= performed; }
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
