using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/Sub Views/LMF Scroll Sub View")]
    public class ScrollSubView : ElementsSubView
    {
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private ViewElement _viewElementPrefab = null;
        private float itemHeight;
        private bool isInitialized;

        private IElementHandler handler;
        private readonly List<object> list = new();
        private readonly List<ViewElement> viewElements = new();
        protected override IReadOnlyList<ViewElement> BlockableViewElements => viewElements;
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

        public void Initialize()
        {
            LMFAssert.NotInitialized(this, isInitialized);
            isInitialized = true;

            itemHeight = _viewElementPrefab.GetComponent<RectTransform>().rect.height;
            _scrollRect.onValueChanged.AddListener((x) => UpdateElements());
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider)
        {
            if (stateProvider == null) { stateProvider = new StateProvider(); }
            if (!(stateProvider is StateProvider local)) throw new System.ArgumentException(
                $"{stateProvider} は {nameof(StateProvider)} ではありません。");

            // 現在の StateProvider を外す前に状態を保存する
            if (currentStateProvider != null)
            {
                currentStateProvider.VerticalAbsolutePosition = VerticalAbsolutePosition;
                currentStateProvider.SelectedIndex = viewElements.IndexOf(LastSelectedViewElement);
            }

            // 表示更新
            this.handler = handler;
            this.list.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                this.list.Add(list[i]);
            }
            SetArg(manager, arg);
            UpdateElements();
            SetStatusCode(0);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            VerticalAbsolutePosition = local.VerticalAbsolutePosition;
            local.ApplySelectedIndex(viewElements);
        }

        private void UpdateElements()
        {
            // ScrollRect の縦幅を埋められる ViewElement の数に変更
            var scrollRectHeight = _scrollRect.viewport.rect.height;
            AdjustViewElements(Mathf.CeilToInt(scrollRectHeight / itemHeight) + 2);

            // 最後の要素が一番上までスクロールできるスライダーサイズに変更
            var contentHeight = scrollRectHeight + itemHeight * (list.Count - 1);
            _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, contentHeight);

            marginHeight = contentHeight - scrollRectHeight;

            // 実際に見えている範囲とその前後1件の ViewElement のみ表示する
            // （前後1件を加えることでUIナビゲーションによる画面外項目の選択が可能となる）
            var elementsOffset = Mathf.FloorToInt(VerticalAbsolutePosition / itemHeight) - 1;
            for (int i = 0; i < viewElements.Count; i++)
            {
                var viewElement = viewElements[i];
                var viewElementTransform = (RectTransform)viewElement.transform;
                var elementIndex = i + elementsOffset;

                // UIナビゲーションに影響するため、非表示の要素も移動させる
                viewElementTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, elementIndex * itemHeight, itemHeight);

                if (elementIndex < 0 || list.Count <= elementIndex)
                {
                    // 範囲外の ViewElement は非表示
                    viewElement.SetVisible(false, true);
                    continue;
                }

                viewElement.SetElement(handler, list[elementIndex]);
                viewElement.SetVisible(true, false);
            }

            // スクロールによって選択中の要素が変わらないよう調整
            var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
            if (selected != null && selected.transform.IsChildOf(_scrollRect.content) && selected.TryGetComponent<ViewElement>(out var selectedElement))
            {
                var selectedIndex = viewElements.IndexOf(selectedElement);
                if (selectedIndex != -1)
                {
                    var i = selectedIndex - elementsOffset + lastItemOffset;
                    if (i < 0 || viewElements.Count <= i) { EventSystem.current.SetSelectedGameObject(null); }
                    else { EventSystem.current.SetSelectedGameObject(viewElements[i].gameObject); }
                }
            }
            lastItemOffset = elementsOffset;
        }

        private void AdjustViewElements(int count)
        {
            if (viewElements.Count != count)
            {
                // 足りない ViewElement を生成する
                while (viewElements.Count < count)
                {
                    var viewElement = Instantiate(_viewElementPrefab, _scrollRect.content.transform);
                    viewElement.Initialize(this);
                    var viewElementTransform = (RectTransform)viewElement.transform;
                    viewElementTransform.anchorMin = new Vector2(0f, 0f);
                    viewElementTransform.anchorMax = new Vector2(1f, 0f);
                    viewElementTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, _scrollRect.content.rect.width);

                    viewElements.Add(viewElement);
                    ViewElement.SetVerticalNavigation(viewElements, viewElements.Count - 1);
                }

                // 不要な ViewElement は削除する
                if (viewElements.Count > count)
                {
                    for (int i = viewElements.Count - 1; i >= count; i--)
                    {
                        Destroy(viewElements[i].gameObject);
                    }
                    viewElements.RemoveRange(count, viewElements.Count - count);
                    ViewElement.SetVerticalNavigation(viewElements, viewElements.Count - 1);
                }
            }
        }

        private void LateUpdate()
        {
            var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
            if (selected == null || !selected.transform.IsChildOf(_scrollRect.content)) return;

            var contentHeight = _scrollRect.content.rect.height;
            var selectedElementTransform = (RectTransform)selected.transform;

            var verticalAbsoluteTop = Mathf.Lerp(-marginHeight, 0f, _scrollRect.verticalNormalizedPosition);
            var verticalAbsoluteBottom = Mathf.Lerp(-contentHeight, -contentHeight + marginHeight, _scrollRect.verticalNormalizedPosition);
            var selectedElementTop = selectedElementTransform.localPosition.y + selectedElementTransform.rect.yMax;
            var selectedElementBottom = selectedElementTransform.localPosition.y + selectedElementTransform.rect.yMin;
            if (selectedElementTop > verticalAbsoluteTop)
            {
                var targetPosition = Mathf.Max(-selectedElementTop, 0f);
                VerticalAbsolutePosition = Mathf.Lerp(VerticalAbsolutePosition, targetPosition, _scrollRect.elasticity);
            }
            else if (selectedElementBottom < verticalAbsoluteBottom)
            {
                var targetPosition = Mathf.Min(
                    -selectedElementBottom - _scrollRect.viewport.rect.height,
                    itemHeight * (viewElements.Count + 1) - _scrollRect.viewport.rect.height);
                VerticalAbsolutePosition = Mathf.Lerp(VerticalAbsolutePosition, targetPosition, _scrollRect.elasticity);
            }
        }

        private class StateProvider : IElementsSubViewStateProvider
        {
            public float VerticalAbsolutePosition { get; set; }
            public int SelectedIndex { get; set; }

            public void Reset()
            {
                VerticalAbsolutePosition = 0f;
                SelectedIndex = -1;
            }

            public void ApplySelectedIndex(List<ViewElement> viewElements)
            {
                if (SelectedIndex <= 0 || viewElements.Count <= SelectedIndex || EventSystem.current == null)
                {
                    // 選択オブジェクトが見つからなければ最初の項目を選択
                    if (viewElements.Count >= 2)
                    {
                        EventSystem.current.SetSelectedGameObject(viewElements[1].gameObject);
                    }
                    return;
                }

                EventSystem.current.SetSelectedGameObject(viewElements[SelectedIndex].gameObject);
            }
        }
    }
}
