using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lysionium.Views
{
    /// <summary>
    /// <see cref="Navigation.Mode.Automatic"/> によるカーソル移動を衝突判定風に制御するコンポーネント。
    /// UIナビゲーションの仕様上このオブジェクトのサイズはカーソル移動に影響しないので注意
    /// </summary>
    [AddComponentMenu("UI/Lysionium/LUI Cursor Collider")]
    [RequireComponent(typeof(Selectable))]
    public class CursorCollider : MonoBehaviour, ISelectHandler
    {
        [Header("Cursor Collision")]
        [Tooltip("このオブジェクトにカーソルが衝突したときの動作を変更する\nfalse: カーソル移動をキャンセル\ntrue: 親 Selectable にカーソル移動先を変更")]
        [SerializeField] private bool _isTrigger = false;

        private Selectable selectable;
        private Selectable parentSelectable;
        private EventSystem eventSystem;
        private SubviewAnimator parentAnimator;
        private bool queuedDisableInteractable;

        protected virtual void Awake()
        {
            selectable = GetComponent<Selectable>();
            parentSelectable = transform.parent.GetComponentInParent<Selectable>();
        }

        protected virtual void Update()
        {
            if (queuedDisableInteractable)
            {
                selectable.interactable = false;
                queuedDisableInteractable = false;
            }

            if (_isTrigger && !selectable.interactable && parentSelectable.IsInteractable())
            {
                if (eventSystem == null)
                {
                    eventSystem = LuiUtility.GetEventSystem(this);
                }

                selectable.interactable = eventSystem.currentSelectedGameObject != parentSelectable;
            }
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (parentAnimator == null)
            {
                // Awake で設定すると早すぎる場合があるのでここで評価
                parentAnimator = GetComponentInParent<SubviewAnimator>();
            }

            if (_isTrigger)
            {
                queuedDisableInteractable = true;
                parentAnimator.QueueSelect(gameObject, parentSelectable.gameObject, CursorEvtfx.Select);
            }
            else
            {
                parentAnimator.QueueSelectToLastSelectedObj(gameObject, CursorEvtfx.None);
            }
        }

        #region Editor Only

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
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
