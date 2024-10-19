using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Cursor Image System")]
    public class CursorImageSystem : MonoBehaviour
    {
        [SerializeField] private Image _cursorPrefab = null;
        [SerializeField] private float _elasticity = .1f;

#if UNITY_EDITOR
        [Space]
        [SerializeField] private GameObject _selectedObj = null;
#endif

        private Image cursorInstance;

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
                var selectedTransform = (RectTransform)eventSystem.currentSelectedGameObject.transform;
                var cursorTransform = (RectTransform)cursorInstance.transform;
                cursorTransform.SetParent(selectedTransform.parent, true);
                cursorTransform.position = Vector3.Lerp(cursorTransform.position, selectedTransform.position, _elasticity);
                cursorTransform.sizeDelta = Vector2.Lerp(cursorTransform.sizeDelta, selectedTransform.rect.size, _elasticity);
                cursorTransform.localScale = selectedTransform.localScale;
                cursorInstance.enabled = true;
            }
            else
            {
                cursorInstance.enabled = false;
            }

#if UNITY_EDITOR
            _selectedObj = eventSystem.currentSelectedGameObject;
#endif
        }
    }
}
