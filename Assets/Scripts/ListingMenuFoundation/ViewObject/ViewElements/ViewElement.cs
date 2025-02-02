using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ListingMF
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ViewElement : MonoBehaviour, ISelectHandler
    {
        private Selectable selectable;
        private CanvasGroup canvasGroup;

        private Navigation notBlockedNavigation;

        /// <summary>
        /// この値が true のとき選択しようとしてもキャンセルする
        /// </summary>
        private bool isOutOfRange;

        /// <summary>
        /// この値が true のときUIナビゲーションによる選択を無効にする
        /// </summary>
        protected bool IsBlocked { get; private set; }

        protected IListMenuManager Manager => Parent.Manager;
        protected IListMenuArg Arg => Parent.Arg;
        protected ElementsSubViewBase Parent { get; private set; }

        public string ElementName { get; private set; }

        public RectTransform RectTransform => (RectTransform)transform;

        public void Initialize(ElementsSubViewBase parent)
        {
            if (parent == null) throw new System.ArgumentNullException(nameof(parent));
            LMFAssert.NotInitialized(this, Parent != null);

            Parent = parent;
            selectable = GetComponent<Selectable>();
            TryGetComponent(out canvasGroup);
            SetBlock(parent.IsBlocked);
        }

        /// <summary>
        /// この要素のUI操作をブロックしてプレイヤーからの操作を防ぐ
        /// </summary>
        public void SetBlock(bool block)
        {
            if (IsBlocked == block) return;

            IsBlocked = block;
            if (selectable == null) return;

            if (block)
            {
                notBlockedNavigation = selectable.navigation;
                selectable.navigation = new Navigation() { mode = Navigation.Mode.None };
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
            else
            {
                selectable.navigation = notBlockedNavigation;
            }
        }

        public void SetElement(object element, IElementHandler handler)
        {
            name = ElementName = handler.GetName(element, Manager, Arg);
            InnerSetElement(element, handler);
        }

        protected abstract void InnerSetElement(object element, IElementHandler handler);

        public void SetVisible(bool visible, bool outOfRange)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
            isOutOfRange = outOfRange;
        }

        void ISelectHandler.OnSelect(BaseEventData eventData) => Parent.OnSelectViewElement(gameObject, isOutOfRange);

        // Animation から呼び出すメソッド
        public void PlayString(string value) => Parent.PlayFromElement(value, this);
        public void PlayObject(Object value) => Parent.PlayFromElement(value, this);



        /// <summary>
        /// 縦並びの <see cref="ViewElement"/> の <see cref="Selectable.navigation"/> を初期化する
        /// </summary>
        internal static void SetVerticalNavigation(IReadOnlyList<ViewElement> viewElements, int index)
        {
            var prevViewElement = index >= 1 ? viewElements[index - 1] : null;
            var centerViewElement = viewElements[index];
            var nextViewElement = index < viewElements.Count - 1 ? viewElements[index + 1] : null;
            var prevSelectable = GetSelectable(prevViewElement);
            var centerSelectable = GetSelectable(centerViewElement);
            var nextSelectable = GetSelectable(nextViewElement);
            if (centerSelectable == null) return;

            if (prevSelectable != null)
            {
                prevViewElement.notBlockedNavigation = prevSelectable.navigation = new Navigation()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = prevSelectable.navigation.selectOnUp,
                    selectOnDown = centerSelectable,
                };
            }

            centerViewElement.notBlockedNavigation = centerSelectable.navigation = new Navigation()
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = prevSelectable,
                selectOnDown = nextSelectable,
            };

            if (nextSelectable != null)
            {
                nextViewElement.notBlockedNavigation = nextSelectable.navigation = new Navigation()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = centerSelectable,
                    selectOnDown = nextSelectable.navigation.selectOnDown,
                };
            }

            Selectable GetSelectable(ViewElement viewElement) => viewElement != null ? viewElement.selectable : null;
        }
    }
}
