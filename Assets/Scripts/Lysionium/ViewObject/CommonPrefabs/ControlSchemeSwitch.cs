using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.Utilities;

namespace Lysionium.Views
{
    /// <summary>
    /// <see cref="PlayerInput"/> のスキームを入力に基づいて自動切り替えするコンポーネント
    /// このコンポーネントを使用する場合 PlayerInput の Auto-Switch は無効化する
    /// 
    /// Auto-Switch は <see cref="OnScreenControl.OnEnable"/> で動作しなくなるため、このコンポーネントが必要
    /// </summary>
    [AddComponentMenu("UI/Lysionium/LUI Control Scheme Switch")]
    [RequireComponent(typeof(PlayerInput))]
    public class ControlSchemeSwitch : MonoBehaviour
    {
        private PlayerInput playerInput;

        private static readonly InternedString onScreenUsage = new("OnScreen");

        protected virtual void Awake()
        {
            TryGetComponent(out playerInput);
        }

        void OnEnable()
        {
            // 全入力デバイスのイベントを監視
            InputSystem.onEvent += OnInputEvent;
        }

        void OnDisable()
        {
            InputSystem.onEvent -= OnInputEvent;
        }

        private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return; // 入力時のみ切り替え
            if (device.usages.Contains(onScreenUsage)) return; // On-Screen Control では切り替えない

            // 先頭から foreach でループさせるため、 InputActions に追加された順で判定することになる
            // そのため、 InputActions に先に追加された ControlScheme が優先して使用される
            foreach (var controlScheme in playerInput.actions.controlSchemes)
            {
                if (!controlScheme.SupportsDevice(device)) continue; // マッチするスキームを先頭から順に探す

                // マッチしたスキームが現在のスキームと同じの場合、スキーム切替なし
                if (controlScheme.name == playerInput.currentControlScheme) return;

                // スキームで使用するデバイスを取得後、スキーム切り替え
                // 必須デバイスが見つからなければこのスキームには切り替えず次の候補へ
                var devices = GetDevicesForControlSchemeOrNull(controlScheme, device);
                if (devices == null) continue;

                // スキーム切り替え確定
                playerInput.SwitchCurrentControlScheme(controlScheme.name, devices);
                break;
            }
        }

        private InputDevice[] GetDevicesForControlSchemeOrNull(InputControlScheme controlScheme, InputDevice triggeringDevice)
        {
            //var matchedDevices = new List<InputDevice>();
            foreach (var requiredDevice in controlScheme.deviceRequirements)
            {
                // 接続中のデバイスからマッチするものを探す
                InputDevice matchedDevice;
                if (InputControlPath.Matches(requiredDevice.controlPath, triggeringDevice))
                {
                    matchedDevice = triggeringDevice;
                }
                else
                {
                    // 現在のイベントを呼び出したデバイスがマッチしない場合、その他の接続済みデバイスから探す
                    // 例:
                    // - Keyboard に対する Mouse
                    // - Touch に対する Gamepad (On-Screen Control 用)
                    // 接続済みデバイスリストの先頭から順に、最初にマッチしたデバイス一つのみを使用する
                    matchedDevice = FirstOrDefault(InputSystem.devices, requiredDevice.controlPath);
                }

                if (matchedDevice != null)
                {
                    //matchedDevices.Add(matchedDevice);

                    // すべてのデバイスを使うパターン
                    //matchedDevices.AddRange(InputSystem.devices.Where(device => InputControlPath.Matches(requiredDevice.controlPath, device)));
                }
                else if (!requiredDevice.isOptional)
                {
                    // 必須デバイスが見つからない場合 null を返す
                    return null;
                }
            }
            //return matchedDevices.ToArray();
            return InputSystem.devices.ToArray();
        }

        private InputDevice FirstOrDefault(ReadOnlyArray<InputDevice> array, string controlPath)
        {
            foreach (var device in array)
            {
                if (InputControlPath.Matches(controlPath, device)) return device;
            }
            return null;
        }
    }
}
