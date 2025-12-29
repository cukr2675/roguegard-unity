using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/Subviews/LUI Scroll Subview")]
    public class ScrollSubview : Subview
    {
        [SerializeField] private ScrollRect _scrollRect = null;

        [SerializeField] private ViewItem _viewItemPrefab = null;

        [Tooltip("カーソル移動に合わせてスクロールする速さ")]
        [SerializeField] private float _elasticityToCursor = 0.2f;

        private float itemHeight;

        private IViewItemHandler handler;
        private readonly List<object> list = new();
        private readonly List<ViewItem> viewItems = new();
        private StateProvider currentStateProvider;

        private int lastItemOffset;

        /// <summary>
        /// スクロールバーの遊び
        /// </summary>
        private float marginHeight;

        private float VerticalAbsolutePosition
        {
            get
            {
                // 後から要素が増えたときのため、スクロール位置を変換したものを返す
                return (1f - _scrollRect.verticalNormalizedPosition) * marginHeight;
            }
            set
            {
                const float epsilon = 1e-4f;
                if (marginHeight < epsilon) { _scrollRect.verticalNormalizedPosition = 0f; } // ゼロ除算対策
                else { _scrollRect.verticalNormalizedPosition = 1f - (value / marginHeight); }
            }
        }

        protected override void CommonInitCore()
        {
            itemHeight = _viewItemPrefab.GetComponent<RectTransform>().rect.height;
            _scrollRect.onValueChanged.AddListener((x) => UpdateViewItems());
            _scrollRect.horizontal = false;
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
                currentStateProvider.VerticalAbsolutePosition = VerticalAbsolutePosition;
                currentStateProvider.SelectedIndex = viewItems.IndexOf(LastSelectedItem);
            }

            // 表示更新
            this.handler = handler;
            this.list.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                this.list.Add(list[i]);
            }
            SetArg(manager, arg);
            InitViewItems();
            UpdateViewItems();
            SetStatusCode(0);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            VerticalAbsolutePosition = local.VerticalAbsolutePosition;
            local.ApplySelectedIndex(this);
        }

        private void InitViewItems()
        {
            for (int i = 0; i < viewItems.Count; i++)
            {
                var viewItem = viewItems[i];
                viewItem.Unbind();
            }
        }

        private void UpdateViewItems()
        {
            // ScrollRect の縦幅を埋められる ViewItem の数に変更
            var scrollRectHeight = _scrollRect.viewport.rect.height;
            AdjustViewItems(Mathf.CeilToInt(scrollRectHeight / itemHeight) + 2); // カーソルスクロール用に上下にはみ出る要素を1つずつ追加

            // 最後の要素が一番上までスクロールできるスライダーサイズに変更
            var contentHeight = scrollRectHeight + itemHeight * (list.Count - 1);
            if (CursorImageSystem.ShowCursor) { contentHeight = itemHeight * list.Count; } // カーソルスクロール中はスクロールバーが余ると変なので要素数と合わせる
            _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, contentHeight);

            marginHeight = contentHeight - scrollRectHeight;

            // 実際に見えている範囲とその前後1件の ViewItem のみ表示する
            // （前後1件を加えることでUIナビゲーションによる画面外項目の選択が可能となる）
            var viewItemsOffset = Mathf.FloorToInt(VerticalAbsolutePosition / itemHeight) - 1;
            for (int i = 0; i < viewItems.Count; i++)
            {
                var viewItem = viewItems[i];
                var viewItemTransform = (RectTransform)viewItem.transform;
                var viewItemIndex = i + viewItemsOffset;

                // UIナビゲーションに影響するため、非表示の要素も移動させる
                viewItemTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, viewItemIndex * itemHeight, itemHeight);

                if (viewItemIndex < 0 || list.Count <= viewItemIndex)
                {
                    // 範囲外の ViewItem は非表示
                    viewItem.Unbind();
                    viewItem.SetVisible(false, true);
                    continue;
                }

                viewItem.Bind(list[viewItemIndex], handler);
                viewItem.SetVisible(true, false);
            }

            // スクロールによって選択中の要素が変わらないよう調整
            var selected = EventSystem != null ? EventSystem.currentSelectedGameObject : null;
            if (selected != null && selected.transform.IsChildOf(_scrollRect.content) && selected.TryGetComponent<ViewItem>(out var selectedViewItem) &&

                Interactable) // 上に表示されているメニューに影響を与えないようにする。これがないとコマンドメニュー表示時の初期選択を上書きしてしまうことがある
            {
                var selectedIndex = viewItems.IndexOf(selectedViewItem);
                if (selectedIndex != -1)
                {
                    var i = selectedIndex - viewItemsOffset + lastItemOffset;
                    if (i < 0 || viewItems.Count <= i)
                    {
                        QueueSelect(gameObject, null, CursorPlay.None);
                    }
                    else
                    {
                        QueueSelect(gameObject, viewItems[i].gameObject, CursorPlay.None);

                        // ↑のようにカーソル移動キューが処理されるのを待っても↓コメントのように直接設定しても選択タイミングは変わらない
                        //EventSystem.SetSelectedGameObject(viewItems[i].gameObject);
                    }
                }
            }
            lastItemOffset = viewItemsOffset;
        }

        private void AdjustViewItems(int count)
        {
            if (viewItems.Count != count)
            {
                // 足りない ViewItem を生成する
                while (viewItems.Count < count)
                {
                    var viewItem = Instantiate(_viewItemPrefab, _scrollRect.content.transform);
                    viewItem.Initialize(this);
                    var viewItemTransform = (RectTransform)viewItem.transform;
                    viewItemTransform.anchorMin = new Vector2(0f, 0f);
                    viewItemTransform.anchorMax = new Vector2(1f, 0f);
                    viewItemTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, _scrollRect.content.rect.width);

                    viewItems.Add(viewItem);
                    ViewItem.SetVerticalNavigation(viewItems, viewItems.Count - 1); // ナビゲーションを明示したほうが長押し移動がスムーズになる？
                }

                // 不要な ViewItem は削除する
                if (viewItems.Count > count)
                {
                    for (int i = viewItems.Count - 1; i >= count; i--)
                    {
                        Destroy(viewItems[i].gameObject);
                    }
                    viewItems.RemoveRange(count, viewItems.Count - count);
                    ViewItem.SetVerticalNavigation(viewItems, viewItems.Count - 1);
                }
            }
        }

        protected virtual void LateUpdate()
        {
            if (!CursorImageSystem.ShowCursor) return;

            // カーソル移動でスクロールする（はみ出ている項目を選択したときスクロールさせる）
            var selected = EventSystem != null ? EventSystem.currentSelectedGameObject : null;
            if (selected == null || !selected.transform.IsChildOf(_scrollRect.content)) return;

            var contentHeight = _scrollRect.content.rect.height;
            var selectedViewItemTransform = (RectTransform)selected.transform;

            var verticalAbsoluteTop = Mathf.Lerp(-marginHeight, 0f, _scrollRect.verticalNormalizedPosition);
            var verticalAbsoluteBottom = Mathf.Lerp(-contentHeight, -contentHeight + marginHeight, _scrollRect.verticalNormalizedPosition);
            var selectedViewItemTop = selectedViewItemTransform.localPosition.y + selectedViewItemTransform.rect.yMax;
            var selectedViewItemBottom = selectedViewItemTransform.localPosition.y + selectedViewItemTransform.rect.yMin;
            if (selectedViewItemTop > verticalAbsoluteTop) // 上にはみ出ているとき
            {
                var targetPosition = Mathf.Max(-selectedViewItemTop, 0f);
                VerticalAbsolutePosition = Mathf.Lerp(VerticalAbsolutePosition, targetPosition, _elasticityToCursor);
            }
            else if (selectedViewItemBottom < verticalAbsoluteBottom) // 下にはみ出ているとき
            {
                var targetPosition = Mathf.Min(-selectedViewItemBottom - _scrollRect.viewport.rect.height, marginHeight);
                VerticalAbsolutePosition = Mathf.Lerp(VerticalAbsolutePosition, targetPosition, _elasticityToCursor);
            }
        }

        private class StateProvider : ISubviewStateProvider
        {
            public float VerticalAbsolutePosition { get; set; }
            public int SelectedIndex { get; set; }

            public void Reset()
            {
                VerticalAbsolutePosition = 0f;
                SelectedIndex = -1;
            }

            public void ApplySelectedIndex(ScrollSubview subview)
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
