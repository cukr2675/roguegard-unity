using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Lysionium
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
            LUIAssert.NotInitialized(this, Parent != null);

            Parent = parent;
            selectable = GetComponent<Selectable>();
            TryGetComponent(out canvasGroup);
        }

        public void SetElement(object element, IElementHandler handler)
        {
            name = ElementName = handler.GetName(element, Manager, Arg);
            InnerSetElement(element, handler);
        }

        public void ClearElement()
        {
            ElementName = null;
            name = "null";
            if (Manager != null) { InnerSetElement(null, ElementToStringHandler.Instance); }
        }

        protected abstract void InnerSetElement(object element, IElementHandler handler);

        public void SetVisible(bool visible, bool outOfRange)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
            isOutOfRange = outOfRange;
        }

        void ISelectHandler.OnSelect(BaseEventData eventData) => OnSelect(eventData);

        protected virtual void OnSelect(BaseEventData eventData)
        {
            try
            {
                if (selectable.IsInteractable()) { Parent.OnSelectViewElement(gameObject, isOutOfRange); }
                else { Parent.QueueSelectToLastSelectedObj(gameObject, CursorPlay.None); }
            }
            catch
            {
                Debug.LogError($"Error from {this}");
                throw;
            }
        }

        // Animation から呼び出すメソッド
        public void PlayString(string value) => Parent.PlayFromElement(value, this);
        public void PlayObject(Object value) => Parent.PlayFromElement(value, this);



        /// <summary>
        /// 指定のリストで最初に出現する <see cref="ViewElement.ElementName"/> != null のインスタンスを取得する
        /// </summary>
        public static bool TryFirstNotNull(IReadOnlyList<ViewElement> viewElements, out ViewElement firstViewElement)
        {
            for (int i = 0; i < viewElements.Count; i++)
            {
                if (viewElements[i].ElementName != null)
                {
                    firstViewElement= viewElements[i];
                    return true;
                }
            }
            firstViewElement = null;
            return false;
        }

        /// <summary>
        /// 縦並びの <see cref="ViewElement"/> の <see cref="Selectable.navigation"/> を設定する
        /// </summary>
        public static void SetVerticalNavigation(IReadOnlyList<ViewElement> viewElements, int index, bool loop = false)
        {
            var prevSelectable = index >= 1 ? viewElements[index - 1].selectable : null;
            var centerSelectable = viewElements[index].selectable;
            var nextSelectable = index < viewElements.Count - 1 ? viewElements[index + 1].selectable : null;
            if (centerSelectable == null) return;

            if (loop)
            {
                if (prevSelectable == null) { prevSelectable = viewElements[viewElements.Count - 1].selectable; }
                if (nextSelectable == null) { nextSelectable = viewElements[0].selectable; }
            }

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

        /// <summary>
        /// 横並びの <see cref="ViewElement"/> の <see cref="Selectable.navigation"/> を設定する
        /// </summary>
        public static void SetHorizontalNavigation(IReadOnlyList<ViewElement> viewElements, int index, bool loop = false)
        {
            var prevSelectable = index >= 1 ? viewElements[index - 1].selectable : null;
            var centerSelectable = viewElements[index].selectable;
            var nextSelectable = index < viewElements.Count - 1 ? viewElements[index + 1].selectable : null;
            if (centerSelectable == null) return;

            if (loop)
            {
                if (prevSelectable == null) { prevSelectable = viewElements[viewElements.Count - 1].selectable; }
                if (nextSelectable == null) { nextSelectable = viewElements[0].selectable; }
            }

            if (prevSelectable != null)
            {
                prevSelectable.navigation = new Navigation()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = prevSelectable.navigation.selectOnLeft,
                    selectOnRight = centerSelectable,
                };
            }

            centerSelectable.navigation = new Navigation()
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = prevSelectable,
                selectOnRight = nextSelectable,
            };

            if (nextSelectable != null)
            {
                nextSelectable.navigation = new Navigation()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = centerSelectable,
                    selectOnRight = nextSelectable.navigation.selectOnRight,
                };
            }
        }
    }
}
