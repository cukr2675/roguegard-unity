using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lysionium
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ViewItem : MonoBehaviour, ISelectHandler
    {
        private Selectable selectable;
        private CanvasGroup canvasGroup;

        /// <summary>
        /// この値が true のとき選択しようとしてもキャンセルする（キャンセル効果音を再生するため選択自体は可能にする）
        /// </summary>
        private bool isOutOfRange;

        protected IListMenuManager Manager => Parent.Manager;
        protected IListMenuArg Arg => Parent.Arg;
        protected SubviewBase Parent { get; private set; }

        public string ItemName { get; private set; }

        public RectTransform RectTransform => (RectTransform)transform;

        public void Initialize(SubviewBase parent)
        {
            if (parent == null) throw new System.ArgumentNullException(nameof(parent));
            LuiAssert.NotInitialized(this, Parent != null);

            Parent = parent;
            selectable = GetComponent<Selectable>();
            TryGetComponent(out canvasGroup);
        }

        public void Bind(object item, IViewItemHandler handler)
        {
            name = ItemName = handler.GetName(item, Manager, Arg);
            BindCore(item, handler);
        }

        public void Unbind()
        {
            ItemName = null;
            name = "null";
            if (Manager != null) { BindCore(null, ToStringViewItemHandler.Instance); }
        }

        protected abstract void BindCore(object item, IViewItemHandler handler);

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
                if (selectable.IsInteractable()) { Parent.OnSelectItem(gameObject, isOutOfRange); }
                else { Parent.QueueSelectToLastSelectedObj(gameObject, CursorPlay.None); }
            }
            catch
            {
                Debug.LogError($"Error from {this}");
                throw;
            }
        }

        // Animation から呼び出すメソッド
        public void PlayString(string value) => Parent.PlayFromItem(value, this);
        public void PlayObject(Object value) => Parent.PlayFromItem(value, this);



        /// <summary>
        /// 指定のリストで最初に出現する <see cref="ItemName"/> != null のインスタンスを取得する
        /// </summary>
        public static bool TryFirstNotNull(IReadOnlyList<ViewItem> viewItems, out ViewItem firstViewItem)
        {
            for (int i = 0; i < viewItems.Count; i++)
            {
                if (viewItems[i].ItemName != null)
                {
                    firstViewItem= viewItems[i];
                    return true;
                }
            }
            firstViewItem = null;
            return false;
        }

        /// <summary>
        /// 縦並びの <see cref="ViewItem"/> の <see cref="Selectable.navigation"/> を設定する
        /// </summary>
        public static void SetVerticalNavigation(IReadOnlyList<ViewItem> viewItems, int index, bool loop = false)
        {
            var prevSelectable = index >= 1 ? viewItems[index - 1].selectable : null;
            var centerSelectable = viewItems[index].selectable;
            var nextSelectable = index < viewItems.Count - 1 ? viewItems[index + 1].selectable : null;
            if (centerSelectable == null) return;

            if (loop)
            {
                if (prevSelectable == null) { prevSelectable = viewItems[viewItems.Count - 1].selectable; }
                if (nextSelectable == null) { nextSelectable = viewItems[0].selectable; }
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
        /// 横並びの <see cref="ViewItem"/> の <see cref="Selectable.navigation"/> を設定する
        /// </summary>
        public static void SetHorizontalNavigation(IReadOnlyList<ViewItem> viewItems, int index, bool loop = false)
        {
            var prevSelectable = index >= 1 ? viewItems[index - 1].selectable : null;
            var centerSelectable = viewItems[index].selectable;
            var nextSelectable = index < viewItems.Count - 1 ? viewItems[index + 1].selectable : null;
            if (centerSelectable == null) return;

            if (loop)
            {
                if (prevSelectable == null) { prevSelectable = viewItems[viewItems.Count - 1].selectable; }
                if (nextSelectable == null) { nextSelectable = viewItems[0].selectable; }
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
