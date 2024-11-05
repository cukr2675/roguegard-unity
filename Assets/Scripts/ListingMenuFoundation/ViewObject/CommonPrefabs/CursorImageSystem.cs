using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Cursor Image System")]
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

        private bool hide;

        private void OnEnable()
        {
            _hideAction.action.performed += OnTouch;
            _hideAction.action.Enable();
            _showAction.action.performed += OnKey;
            _showAction.action.Enable();
        }

        private void OnDisable()
        {
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

        private void Update()
        {
            if (!cursorInstance)
            {
                // カーソルオブジェクトが削除されたら再生成する
                cursorInstance = Instantiate(_cursorPrefab);
            }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) return;

            if (eventSystem.currentSelectedGameObject != null)
            {
                var deltaElasticity = 1f - _elasticity / Time.deltaTime;
                var selectedTransform = (RectTransform)eventSystem.currentSelectedGameObject.transform;
                var cursorTransform = (RectTransform)cursorInstance.transform;
                cursorTransform.SetParent(selectedTransform.parent, true);
                cursorTransform.position = Vector3.Lerp(cursorTransform.position, selectedTransform.position, deltaElasticity);
                cursorTransform.sizeDelta = Vector2.Lerp(cursorTransform.sizeDelta, selectedTransform.rect.size, deltaElasticity);
                cursorTransform.localScale = selectedTransform.localScale;
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
    }
}
