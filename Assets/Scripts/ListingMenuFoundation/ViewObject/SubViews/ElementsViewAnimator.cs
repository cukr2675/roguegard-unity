using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Elements View Animator")]
    public class ElementsViewAnimator : MonoBehaviour
    {
        [SerializeField] private string _visibleBool = "IsVisible";
        public string VisibleBool => _visibleBool;

        [SerializeField] private string _statusCodeInteger = "StatusCode";
        public string StatusCodeInteger => _statusCodeInteger;

        [Space]
        [SerializeField] private PlayStringEvent _onPlayString = null;
        public PlayStringEvent OnPlayString => _onPlayString;

        [Space]
        [SerializeField] private PlayObjectEvent _onPlayObject = null;
        public PlayObjectEvent OnPlayObject => _onPlayObject;

        [Header("ViewElement")]

        [SerializeField] private string _playOnSelect = "Select";

        [SerializeField] private string _playOnSelectOutOfRange = "SelectOutOfRange";

        [SerializeField] private bool _cancelOnSelectOutOfRange = true;

#if UNITY_EDITOR
        [Header("Debug (Editor Only)")]
        [SerializeField] private bool _log = false;
#endif

        private GameObject prevSelectedGameObject;
        private bool queuedCancelSelection;

        public static ElementsViewAnimator Get(Component obj)
        {
            LMFUtility.TryGetComponentInRecursiveParents<ElementsViewAnimator>(obj.transform, out var viewAnimator);
            return viewAnimator;
        }

        private void Awake()
        {
#if UNITY_EDITOR
            if (_log)
            {
                _onPlayString.AddListener(Debug.Log);
                _onPlayObject.AddListener(Debug.Log);
            }
#endif
        }

        private void Update()
        {
            if (queuedCancelSelection)
            {
                EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
                queuedCancelSelection = false;
            }
        }

        public void OnSelect(GameObject gameObject, bool outOfRange)
        {
            // 項目選択時の Play を実行（選択をキャンセル中は何もしない）
            if (!queuedCancelSelection)
            {
                if (outOfRange)
                {
                    _onPlayString.Invoke(_playOnSelectOutOfRange);
                }
                else
                {
                    _onPlayString.Invoke(_playOnSelect);
                }
            }

            // 範囲外の項目を選択したとき選択をキャンセルする
            if (_cancelOnSelectOutOfRange && outOfRange)
            {
                queuedCancelSelection = true;
            }
            else
            {
                prevSelectedGameObject = gameObject;
            }
        }

        [System.Serializable] public class PlayStringEvent : UnityEvent<string> { }
        [System.Serializable] public class PlayObjectEvent : UnityEvent<string> { }
    }
}
