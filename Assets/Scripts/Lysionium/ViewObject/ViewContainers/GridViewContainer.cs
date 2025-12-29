using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/View Containers/LUI Grid View Container")]
    public class GridViewContainer : MonoBehaviour
    {
        [SerializeField] private RectTransform _content = null;
        [SerializeField] private DropdownButtonViewItem _viewItemPrefab = null;
        private Vector2 itemSize;
        private Vector2 margin;
        private bool isInitialized;

        [Header("Layout")]

        [Tooltip("グリッド分割数")]
        [SerializeField] private Vector2Int _viewItemCount = Vector2Int.one;

        [Tooltip("拡張方向（はい/いいえ等の選択肢用）")]
        [SerializeField] private ExtensionDirection _extensionDirection = ExtensionDirection.NotExtend;

        [Header("Other")]

        [Tooltip("この値が true のときカーソル移動の対象となる")]
        [SerializeField] private bool _isSelectable = true;

        private readonly List<DropdownButtonViewItem> _viewItems = new();
        public IReadOnlyList<ViewItem> ViewItems => _viewItems;

        private void Initialize()
        {
            var contentRectTransform = (RectTransform)_content.transform;
            var rect = contentRectTransform.rect;
            itemSize = rect.size / _viewItemCount;

            var rectTransform = (RectTransform)transform;
            margin = rectTransform.rect.size - rect.size;

            isInitialized = true;
        }

        public void SetListHandler(IReadOnlyList<object> list, IViewItemHandler handler, Subview subview, Rect rect)
        {
            if (!isInitialized) { Initialize(); }

            // 表示要素を生成/削除
            if (_extensionDirection != ExtensionDirection.NotExtend)
            {
                AdjustViewItems(list.Count, handler, subview);
            }
            else
            {
                var length = _viewItemCount.x * _viewItemCount.y;
                length = Mathf.Min(length, list.Count);
                AdjustViewItems(length, handler, subview);
            }

            // 表示要素に値とハンドラを注入
            for (int i = 0; i < _viewItems.Count; i++)
            {
                var itemButton = _viewItems[i];
                itemButton.Bind(list[i], handler);
            }
        }

        private void AdjustViewItems(int count, IViewItemHandler handler, Subview subview)
        {
            if (_viewItems.Count == count) return;

            // 足りない ViewItem を生成する
            while (_viewItems.Count < count)
            {
                var index = _viewItems.Count;
                var x = index % _viewItemCount.x;
                var y = index / _viewItemCount.x;

                var viewItem = Instantiate(_viewItemPrefab, _content.transform);
                viewItem.Initialize(subview, (children, rect) =>
                {
                    var childContainer = Instantiate(this, subview.transform);
                    childContainer.SetListHandler(children, handler, subview, rect);
                });
                viewItem.SetVisible(true, false);
                if (viewItem.TryGetComponent<Selectable>(out var selectable))
                {
                    selectable.navigation = new Navigation() { mode = _isSelectable ? Navigation.Mode.Automatic : Navigation.Mode.None };
                }
                var itemButtonTransform = (RectTransform)viewItem.transform;
                itemButtonTransform.anchorMin = new Vector2(0f, 0f);
                itemButtonTransform.anchorMax = new Vector2(1f, 0f);
                itemButtonTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x * itemSize.x, itemSize.x);
                itemButtonTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y * itemSize.y, itemSize.y);

                _viewItems.Add(viewItem);
                //ViewItem.SetVerticalNavigation(index >= 1 ? viewItems[index - 1] : null, viewItem);
            }

            // 不要な ViewItem は削除する
            if (_viewItems.Count > count)
            {
                for (int i = _viewItems.Count - 1; i >= count; i--)
                {
                    Destroy(_viewItems[i].gameObject);
                }
                _viewItems.RemoveRange(count, _viewItems.Count - count);
            }

            // 選択肢の要素数に合わせてウィンドウを拡張する
            if (_extensionDirection == ExtensionDirection.Up)
            {
                var rectTransform = (RectTransform)transform;
                rectTransform.sizeDelta = new Vector2(itemSize.x, itemSize.y * count) + margin;
            }
        }

        public void SetPosition(Rect rect)
        {
            var rectTransform = (RectTransform)transform;
            rectTransform.sizeDelta = rect.size;
            rectTransform.anchoredPosition = rect.position;
        }

        #region Editor Only

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            if (_content == null) return;

            // ボタン配置グリッドプレビュー
            var contentTransform = _content.transform;
            var rect = ((RectTransform)contentTransform).rect;
            var size = rect.size * contentTransform.lossyScale;
            var startPosition = (Vector2)contentTransform.position - size / 2f;
            var itemSize = size / _viewItemCount;
            Gizmos.color = Color.green;
            for (int y = 0; y < _viewItemCount.y; y++)
            {
                for (int x = 0; x < _viewItemCount.x; x++)
                {
                    var position = startPosition + itemSize * new Vector2(x + 0.5f, y + 0.5f);
                    Gizmos.DrawWireCube(position, itemSize);
                }
            }
        }
#endif

        #endregion

        private enum ExtensionDirection
        {
            NotExtend,
            Up,
        }

    }
}
