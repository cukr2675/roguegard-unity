using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/Sub Views/LMF Grid Sub View")]
    public class GridSubView : ElementsSubView
    {
        [SerializeField] private RectTransform _content = null;
        [SerializeField] private ViewElement _viewElementPrefab = null;
        private Vector2 itemSize;
        private Vector2 margin;

        [Header("Layout")]
        [SerializeField] private Vector2Int _viewElementCount = Vector2Int.one;
        [SerializeField] private ExtensionDirection _extensionDirection = ExtensionDirection.NotExtend;

        [Header("Other")]
        [SerializeField] private bool _isSelectable = true;
        private bool isInitialized;

        private IElementHandler handler;
        private readonly List<ViewElement> viewElements = new();
        protected override IReadOnlyList<ViewElement> BlockableViewElements => viewElements;
        private StateProvider currentStateProvider;

        public void Initialize()
        {
            LMFAssert.NotInitialized(this, isInitialized);
            isInitialized = true;

            var contentRectTransform = (RectTransform)_content.transform;
            var rect = contentRectTransform.rect;
            itemSize = rect.size / _viewElementCount;

            var rectTransform = (RectTransform)transform;
            margin = rectTransform.rect.size - rect.size;
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
                currentStateProvider.SelectedIndex = viewElements.IndexOf(LastSelectedViewElement);
            }

            this.handler = handler;
            SetArg(manager, arg);
            UpdateElements(list);
            SetStatusCode(0);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            if (_isSelectable) { local.ApplySelectedIndex(viewElements); }
        }

        private void UpdateElements(IReadOnlyList<object> list)
        {
            if (_extensionDirection != ExtensionDirection.NotExtend)
            {
                AdjustViewElements(list.Count);
            }
            else
            {
                var length = _viewElementCount.x * _viewElementCount.y;
                length = Mathf.Min(length, list.Count);
                AdjustViewElements(length);
            }

            for (int i = 0; i < viewElements.Count; i++)
            {
                var itemButton = viewElements[i];
                itemButton.SetElement(handler, list[i]);
            }
        }

        private void AdjustViewElements(int count)
        {
            if (viewElements.Count != count)
            {
                // 足りない ViewElement を生成する
                while (viewElements.Count < count)
                {
                    var index = viewElements.Count;
                    var x = index % _viewElementCount.x;
                    var y = index / _viewElementCount.x;

                    var viewElement = Instantiate(_viewElementPrefab, _content.transform);
                    viewElement.Initialize(this);
                    viewElement.SetVisible(true, !_isSelectable);
                    var itemButtonTransform = (RectTransform)viewElement.transform;
                    itemButtonTransform.anchorMin = new Vector2(0f, 0f);
                    itemButtonTransform.anchorMax = new Vector2(1f, 0f);
                    itemButtonTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x * itemSize.x, itemSize.x);
                    itemButtonTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y * itemSize.y, itemSize.y);

                    viewElements.Add(viewElement);
                    //ViewElement.SetVerticalNavigation(index >= 1 ? viewElements[index - 1] : null, viewElement);
                }

                // 不要な ViewElement は削除する
                if (viewElements.Count > count)
                {
                    for (int i = viewElements.Count - 1; i >= count; i--)
                    {
                        Destroy(viewElements[i].gameObject);
                    }
                    viewElements.RemoveRange(count, viewElements.Count - count);
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
        private void OnDrawGizmosSelected()
        {
            if (_content == null) return;

            var contentTransform = _content.transform;
            var rect = ((RectTransform)contentTransform).rect;
            var size = rect.size * contentTransform.lossyScale;
            var startPosition = (Vector2)contentTransform.position - size / 2f;
            var itemSize = size / _viewElementCount;
            Gizmos.color = Color.green;
            for (int y = 0; y < _viewElementCount.y; y++)
            {
                for (int x = 0; x < _viewElementCount.x; x++)
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

        private class StateProvider : IElementsSubViewStateProvider
        {
            public int SelectedIndex { get; set; }

            public void Reset()
            {
                SelectedIndex = -1;
            }

            public void ApplySelectedIndex(List<ViewElement> viewElements)
            {
                if (SelectedIndex <= 0 || viewElements.Count <= SelectedIndex || EventSystem.current == null)
                {
                    // 選択オブジェクトが見つからなければ最初の項目を選択
                    if (viewElements.Count >= 1)
                    {
                        EventSystem.current.SetSelectedGameObject(viewElements[0].gameObject);
                    }
                    return;
                }

                EventSystem.current.SetSelectedGameObject(viewElements[SelectedIndex].gameObject);
            }
        }
    }
}
