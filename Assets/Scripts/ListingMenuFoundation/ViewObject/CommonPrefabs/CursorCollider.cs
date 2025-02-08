using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ListingMF
{
    /// <summary>
    /// <see cref="Navigation.Mode.Automatic"/> によるカーソル移動を衝突判定風に制御するコンポーネント。
    /// UIナビゲーションの仕様上このオブジェクトのサイズはカーソル移動に影響しないので注意
    /// </summary>
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Cursor Collider")]
    [RequireComponent(typeof(Selectable))]
    public class CursorCollider : MonoBehaviour, ISelectHandler
    {
        [Header("Cursor Collision")]
        [Tooltip("このオブジェクトにカーソルが衝突したときの動作を変更する\nfalse: カーソル移動をキャンセル\ntrue: 親 Selectable にカーソル移動先を変更")]
        [SerializeField] private bool _isTrigger = false;

        private Selectable selectable;
        private Selectable parentSelectable;
        private ElementsViewAnimator parentAnimator;
        private bool queuedDisableInteractable;

        private void Awake()
        {
            selectable = GetComponent<Selectable>();
            LMFUtility.TryGetComponentInRecursiveParents(transform.parent, out parentSelectable);
        }

        private void Update()
        {
            if (queuedDisableInteractable)
            {
                selectable.interactable = false;
                queuedDisableInteractable = false;
            }

            if (_isTrigger && !selectable.interactable && parentSelectable.IsInteractable())
            {
                selectable.interactable = EventSystem.current.currentSelectedGameObject != parentSelectable;
            }
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (parentAnimator == null)
            {
                // Awake で設定すると早すぎる場合があるのでここで評価
                LMFUtility.TryGetComponentInRecursiveParents(transform, out parentAnimator);
            }

            if (_isTrigger)
            {
                queuedDisableInteractable = true;
                parentAnimator.QueueSelect(gameObject, parentSelectable.gameObject, CursorPlay.Select);
            }
            else
            {
                parentAnimator.QueueSelectToLastSelectedObj(gameObject, CursorPlay.None);
            }
        }

        #region Editor Only

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!UnityEditor.EditorPrefs.GetBool("SelectableEditor.ShowNavigation")) return;
            if (UnityEditor.Selection.activeGameObject == null) return;
            if (!UnityEditor.Selection.activeGameObject.GetComponent<Selectable>()) return;

            var rect = ((RectTransform)transform).rect;
            var center = rect.center * transform.lossyScale;
            var position = (Vector2)transform.position + center;
            Gizmos.color = new Color(0f, 1f, 0f, .25f);
            Gizmos.matrix = Matrix4x4.Translate(position) * Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, 45f));
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 25f); // サイズは適当
        }
#endif

        #endregion
    }
}
