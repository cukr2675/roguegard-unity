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

        /// <summary>
        /// この値が true のとき選択しようとしてもキャンセルする（キャンセル効果音を再生するため選択自体は可能にする）
        /// </summary>
        private bool isOutOfRange;

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
        }

        public void SetElement(object element, IElementHandler handler)
        {
            name = ElementName = handler.GetName(element, Manager, Arg);
            InnerSetElement(element, handler);
        }

        protected abstract void InnerSetElement(object element, IElementHandler handler);

        public void ClearElementName()
        {
            ElementName = null;
            name = "null";
        }

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
        /// 縦並びの <see cref="ViewElement"/> の <see cref="Selectable.navigation"/> を設定する
        /// </summary>
        public static void SetVerticalNavigation(IReadOnlyList<ViewElement> viewElements, int index)
        {
            var prevSelectable = index >= 1 ? viewElements[index - 1].selectable : null;
            var centerSelectable = viewElements[index].selectable;
            var nextSelectable = index < viewElements.Count - 1 ? viewElements[index + 1].selectable : null;
            if (centerSelectable == null) return;

            if (prevSelectable != null)
            {
                prevSelectable.navigation = new Navigation()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = prevSelectable.navigation.selectOnUp,
                    selectOnDown = centerSelectable,
                };
            }

            centerSelectable.navigation = new Navigation()
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = prevSelectable,
                selectOnDown = nextSelectable,
            };

            if (nextSelectable != null)
            {
                nextSelectable.navigation = new Navigation()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = centerSelectable,
                    selectOnDown = nextSelectable.navigation.selectOnDown,
                };
            }
        }
    }
}
