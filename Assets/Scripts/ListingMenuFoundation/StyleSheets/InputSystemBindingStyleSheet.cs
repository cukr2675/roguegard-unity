using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.InputSystem;

namespace ListingMF
{
    /// <summary>
    /// 要素のスタイルでキーバインドするスタイルシート。このオブジェクトの下の <see cref="ElementsSubView"/> に影響を与える
    /// </summary>
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Input System Binding Style Sheet")]
    public class InputSystemBindingStyleSheet : MonoBehaviour
    {
        [SerializeField] private Binding[] _bindings = null;

        public static InputSystemBindingStyleSheet Get(Component obj)
        {
            LMFUtility.TryGetComponentInRecursiveParents<InputSystemBindingStyleSheet>(obj.transform, out var viewAnimator);
            return viewAnimator;
        }

        private bool TryGetAction(ReadOnlySpan<char> style, out InputAction action)
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

        public void Bind(ReadOnlySpan<char> style, Action<InputAction.CallbackContext> performed)
        {
            if (TryGetAction(style, out var action)) return;

            action.performed += performed;
            action.Enable();
        }

        public void Unbind(ReadOnlySpan<char> style, Action<InputAction.CallbackContext> performed)
        {
            if (TryGetAction(style, out var action)) return;

            action.performed -= performed;
            //action.Disable(); // バインディングされているアクションが一つとは限らないため無効化しない
        }

        [Serializable]
        private class Binding
        {
            [SerializeField] private string _style;
            public ReadOnlySpan<char> Style => MemoryExtensions.AsSpan(_style);

            [SerializeField] private InputAction _action;
            public InputAction Action => _action;
        }
    }
}
