using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/LUI Cursor Image System")]
    public class CursorImageSystem : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _cursorPrefab = null;
        [SerializeField] private float _elasticity = .01f;

        [SerializeField] private InputActionReference _hideAction = null;
        [SerializeField] private InputActionReference _showAction = null;

#if UNITY_EDITOR
        [Space]
        [SerializeField] private GameObject _selectedObj = null;
#endif

        private CanvasGroup cursorInstance;
        private EventSystem eventSystem;

        private bool hide;

        private static CursorImageSystem current;
        public static bool ShowCursor => !current.hide;

        protected virtual void OnEnable()
        {
            current = this;
            _hideAction.action.performed += OnTouch;
            _hideAction.action.Enable();
            _showAction.action.performed += OnKey;
            _showAction.action.Enable();
        }

        protected virtual void OnDisable()
        {
            if (current == this) { current = null; }
            _hideAction.action.performed -= OnTouch;
            _hideAction.action.Disable();
            _showAction.action.performed -= OnKey;
            _showAction.action.Disable();
        }

        private void OnTouch(InputAction.CallbackContext context)
        {
            hide = true;
        }

        private void OnKey(InputAction.CallbackContext context)
        {
            hide = false;
        }

        protected virtual void Update()
        {
            if (!cursorInstance)
            {
                // カーソルオブジェクトが削除されたら再生成する
                cursorInstance = Instantiate(_cursorPrefab);
            }
            if (!eventSystem)
            {
                eventSystem = LuiUtility.GetEventSystem(this);
            }

            if (eventSystem.currentSelectedGameObject != null)
            {
                var deltaElasticity = 1f - _elasticity / Time.deltaTime;
                var selectedTransform = (RectTransform)eventSystem.currentSelectedGameObject.transform;
                var cursorTransform = (RectTransform)cursorInstance.transform;
                var relativeScale = (Vector2)selectedTransform.lossyScale;
                relativeScale.x /= cursorTransform.lossyScale.x;
                relativeScale.y /= cursorTransform.lossyScale.y;
                cursorTransform.SetParent(GetUncontrolledParent(selectedTransform.parent), true);
                var targetPosition = selectedTransform.position + (Vector3)(selectedTransform.rect.center * relativeScale / 2f);
                targetPosition = Vector3.Lerp(cursorTransform.position, targetPosition, deltaElasticity);
                var targetSizeDelta = Vector2.Lerp(cursorTransform.sizeDelta, selectedTransform.rect.size * relativeScale, deltaElasticity);
                if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.y) || float.IsNaN(targetPosition.z) ||
                    float.IsNaN(targetSizeDelta.x) || float.IsNaN(targetSizeDelta.y))
                {
                    Debug.LogWarning($"不正な項目 {eventSystem.currentSelectedGameObject} が選択されました。");
                    eventSystem.SetSelectedGameObject(null);
                    Destroy(cursorInstance.gameObject);
                    return;
                }
                cursorTransform.position = targetPosition;
                cursorTransform.sizeDelta = targetSizeDelta;
                cursorTransform.localScale = _cursorPrefab.transform.localScale;
                cursorInstance.alpha = hide ? 0f : 1f;
            }
            else
            {
                cursorInstance.alpha = 0f;
            }

#if UNITY_EDITOR
            _selectedObj = eventSystem.currentSelectedGameObject;
#endif
        }

        /// <summary>
        /// <see cref="LayoutGroup"/> に制御されない直近の親を取得する
        /// </summary>
        private static Transform GetUncontrolledParent(Transform transform)
        {
            while (LuiUtility.TryGetComponentInParent<LayoutGroup>(transform, out var layoutGroup))
            {
                // 親にコントロールを制御するコンポーネントが存在する場合、そのコンポーネント上を対象として再検証
                transform = layoutGroup.transform.parent;
            }

            // 親にコントロールを制御するコンポーネントが存在しなければ、この Transform 直下に移動する
            return transform;
        }
    }
}