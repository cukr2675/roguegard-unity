using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

namespace Lysionium.Views
{
    /// <summary>
    /// <see cref="Navigation.Mode.Automatic"/> によるカーソル移動を衝突判定風に制御するコンポーネント。
    /// UIナビゲーションの仕様上このオブジェクトのサイズはカーソル移動に影響しないので注意
    /// </summary>
    [AddComponentMenu("UI/Lysionium/LUI Control Schemed Canvas Switch")]
    public class ControlSchemedCanvasSwitch : MonoBehaviour
    {
        [SerializeField] private CanvasElement[] _items;

        private PlayerInput playerInput;

        protected virtual void OnEnable()
        {
            if (playerInput == null) { playerInput = GetComponentInParent<PlayerInput>(); }
            if (playerInput == null) return;

            InputUser.onChange += OnChange;

            // 初期状態のUIを設定
            OnControlsChanged(playerInput.currentControlScheme);
        }

        protected virtual void OnDisable()
        {
            InputUser.onChange -= OnChange;
        }

        private void OnChange(InputUser user, InputUserChange change, InputDevice device)
        {
            if (user != playerInput.user) return;

            if (change == InputUserChange.ControlSchemeChanged)
            {
                OnControlsChanged(playerInput.currentControlScheme);
            }
        }

        private void OnControlsChanged(string controlScheme)
        {
            var firstEnabledElement = true;
            foreach (var item in _items)
            {
                var enabled = string.IsNullOrWhiteSpace(item.ControlScheme) || item.ControlScheme == controlScheme;
                enabled &= firstEnabledElement; // 最初に有効判定となった要素だけが有効
                item.Canvas.enabled = enabled;

                if (firstEnabledElement && enabled)
                {
                    firstEnabledElement = false;
                }
            }
        }

        [System.Serializable]
        private class CanvasElement
        {
            [Tooltip("空白のときすべての ControlScheme とマッチする")]
            [SerializeField] private string _controlScheme;
            public string ControlScheme => _controlScheme;

            [SerializeField] private Canvas _canvas;
            public Canvas Canvas => _canvas;
        }
    }
}
