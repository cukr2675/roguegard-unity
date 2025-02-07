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
    public class CursorCollider : Selectable
    {
        [Header("Cursor Collision")]
        [Tooltip("このオブジェクトにカーソルが衝突したときの動作を変更する\nfalse: カーソル移動をキャンセル\ntrue: 親 Selectable にカーソル移動先を変更")]
        [SerializeField] private bool _isTrigger = false;

        private Selectable parentSelectable;
        private ElementsViewAnimator parentAnimator;
        private bool queuedDisableInteractable;

        protected override void Awake()
        {
            base.Awake();
            LMFUtility.TryGetComponentInRecursiveParents(transform.parent, out parentSelectable);
            LMFUtility.TryGetComponentInRecursiveParents(transform, out parentAnimator);
        }

        private void Update()
        {
            if (queuedDisableInteractable)
            {
                interactable = false;
                queuedDisableInteractable = false;
            }

            if (!interactable && parentSelectable.IsInteractable())
            {
                interactable = EventSystem.current.currentSelectedGameObject != parentSelectable;
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
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
    }
}
