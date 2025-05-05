using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/Subviews/LUI Grid Subview")]
    public class GridSubview : ElementsSubview
    {
        [SerializeField] private RectTransform _content = null;
        [SerializeField] private ViewElement _viewElementPrefab = null;
        private Vector2 itemSize;
        private Vector2 margin;

        [Header("Layout")]

        [Tooltip("グリッド分割数")]
        [SerializeField] private Vector2Int _viewElementCount = Vector2Int.one;

        [Tooltip("拡張方向（はい/いいえ等の選択肢用）")]
        [SerializeField] private ExtensionDirection _extensionDirection = ExtensionDirection.NotExtend;

        [Header("Other")]

        [Tooltip("この値が true のときカーソル移動の対象となる")]
        [SerializeField] private bool _isSelectable = true;

        private IElementHandler handler;
        private readonly List<ViewElement> viewElements = new();
        private StateProvider currentStateProvider;

        protected override void CommonInitCore()
        {
            var contentRectTransform = (RectTransform)_content.transform;
            var rect = contentRectTransform.rect;
            itemSize = rect.size / _viewElementCount;

            var rectTransform = (RectTransform)transform;
            margin = rectTransform.rect.size - rect.size;
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubviewStateProvider stateProvider)
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
            if (_isSelectable) { local.ApplySelectedIndex(this); }
        }

        private void UpdateElements(IReadOnlyList<object> list)
        {
            // 表示要素を生成/削除
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

            // 表示要素に値とハンドラを注入
            for (int i = 0; i < viewElements.Count; i++)
            {
                var itemButton = viewElements[i];
                itemButton.SetElement(list[i], handler);
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
                    viewElement.SetVisible(true, false);
                    if (viewElement.TryGetComponent<Selectable>(out var selectable))
                    {
                        selectable.navigation = new Navigation() { mode = _isSelectable ? Navigation.Mode.Automatic : Navigation.Mode.None };
                    }
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

            // ボタン配置グリッドプレビュー
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

        private class StateProvider : IElementsSubviewStateProvider
        {
            public int SelectedIndex { get; set; }

            public void Reset()
            {
                SelectedIndex = -1;
            }

            public void ApplySelectedIndex(GridSubview subview)
            {
                if (SelectedIndex <= 0 || subview.viewElements.Count <= SelectedIndex || EventSystem.current == null)
                {
                    // 選択オブジェクトが見つからなければ最初の項目を選択
                    if (ViewElement.TryFirstNotNull(subview.viewElements, out var first))
                    {
                        //EventSystem.current.SetSelectedGameObject(first.gameObject); // これだと Show メソッドで interactable が true になる前に選択してしまう
                        subview.QueueSelect(subview.gameObject, first.gameObject, CursorPlay.None);
                    }
                    return;
                }

                subview.QueueSelect(subview.gameObject, subview.viewElements[SelectedIndex].gameObject, CursorPlay.None);
            }
        }
    }
}
