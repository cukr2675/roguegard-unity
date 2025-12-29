using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/Subviews/LUI Grid Subview")]
    public class GridSubview : Subview
    {
        [SerializeField] private RectTransform _content = null;
        [SerializeField] private ViewItem _viewItemPrefab = null;
        private Vector2 itemSize;
        private Vector2 margin;

        [Header("Layout")]

        [Tooltip("グリッド分割数")]
        [SerializeField] private Vector2Int _viewItemCount = Vector2Int.one;

        [Tooltip("拡張方向（はい/いいえ等の選択肢用）")]
        [SerializeField] private ExtensionDirection _extensionDirection = ExtensionDirection.NotExtend;

        [Header("Other")]

        [Tooltip("この値が true のときカーソル移動の対象となる")]
        [SerializeField] private bool _isSelectable = true;

        private IViewItemHandler handler;
        private readonly List<ViewItem> viewItems = new();
        private StateProvider currentStateProvider;

        protected override void CommonInitCore()
        {
            var contentRectTransform = (RectTransform)_content.transform;
            var rect = contentRectTransform.rect;
            itemSize = rect.size / _viewItemCount;

            var rectTransform = (RectTransform)transform;
            margin = rectTransform.rect.size - rect.size;
        }

        public override void SetListHandler(
            IReadOnlyList<object> list, IViewItemHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref ISubviewStateProvider stateProvider)
        {
            stateProvider ??= new StateProvider();
            if (stateProvider is not StateProvider local) throw new System.ArgumentException(
                $"{stateProvider} は {nameof(StateProvider)} ではありません。");

            // 現在の StateProvider を外す前に状態を保存する
            if (currentStateProvider != null)
            {
                currentStateProvider.SelectedIndex = viewItems.IndexOf(LastSelectedItem);
            }

            this.handler = handler;
            SetArg(manager, arg);
            UpdateViewItems(list);
            SetStatusCode(0);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            if (_isSelectable) { local.ApplySelectedIndex(this); }
        }

        private void UpdateViewItems(IReadOnlyList<object> list)
        {
            // 表示要素を生成/削除
            if (_extensionDirection != ExtensionDirection.NotExtend)
            {
                AdjustViewItems(list.Count);
            }
            else
            {
                var length = _viewItemCount.x * _viewItemCount.y;
                length = Mathf.Min(length, list.Count);
                AdjustViewItems(length);
            }

            // 表示要素に値とハンドラを注入
            for (int i = 0; i < viewItems.Count; i++)
            {
                var itemButton = viewItems[i];
                itemButton.Bind(list[i], handler);
            }
        }

        private void AdjustViewItems(int count)
        {
            if (viewItems.Count != count)
            {
                // 足りない ViewItem を生成する
                while (viewItems.Count < count)
                {
                    var index = viewItems.Count;
                    var x = index % _viewItemCount.x;
                    var y = index / _viewItemCount.x;

                    var viewItem = Instantiate(_viewItemPrefab, _content.transform);
                    viewItem.Initialize(this);
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

                    viewItems.Add(viewItem);
                    //ViewItem.SetVerticalNavigation(index >= 1 ? viewItems[index - 1] : null, viewItem);
                }

                // 不要な ViewItem は削除する
                if (viewItems.Count > count)
                {
                    for (int i = viewItems.Count - 1; i >= count; i--)
                    {
                        Destroy(viewItems[i].gameObject);
                    }
                    viewItems.RemoveRange(count, viewItems.Count - count);
                }

                // 選択肢の要素数に合わせてウィンドウを拡張する
                if (_extensionDirection == ExtensionDirection.Up)
                {
                    var rectTransform = (RectTransform)transform;
                    rectTransform.sizeDelta = new Vector2(itemSize.x, itemSize.y * count) + margin;
                }
            }
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

        private class StateProvider : ISubviewStateProvider
        {
            public int SelectedIndex { get; set; }

            public void Reset()
            {
                SelectedIndex = -1;
            }

            public void ApplySelectedIndex(GridSubview subview)
            {
                if (SelectedIndex <= 0 || subview.viewItems.Count <= SelectedIndex || subview.EventSystem == null)
                {
                    // 選択オブジェクトが見つからなければ最初の項目を選択
                    if (ViewItem.TryFirstNotNull(subview.viewItems, out var first))
                    {
                        //subview.EventSystem.SetSelectedGameObject(first.gameObject); // これだと Show メソッドで interactable が true になる前に選択してしまう
                        subview.QueueSelect(subview.gameObject, first.gameObject, CursorPlay.None);
                    }
                    return;
                }

                subview.QueueSelect(subview.gameObject, subview.viewItems[SelectedIndex].gameObject, CursorPlay.None);
            }
        }
    }
}
