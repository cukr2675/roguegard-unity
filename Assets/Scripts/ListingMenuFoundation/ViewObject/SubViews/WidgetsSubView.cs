using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/Sub Views/LMF Widgets Sub View")]
    public class WidgetsSubView : ElementsSubView
    {
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField, Tooltip("Awake で起動するトリガー名　サブビューを表示状態で生成したいときに使う")] private string _initTrigger = null;

        [Header("Other")]
        [SerializeField] private bool _isSelectable = true;

        private IElementHandler handler;
        private readonly List<ViewElement> viewElements = new();
        protected override IReadOnlyList<ViewElement> BlockableViewElements => viewElements;
        private readonly List<ViewWidget> viewWidgets = new();
        private readonly List<Selectable> selectables = new();
        private readonly List<GameObject> viewWidgetGameObjects = new();
        private StateProvider currentStateProvider;

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

        private void Awake()
        {
            if (!string.IsNullOrWhiteSpace(_initTrigger) && TryGetComponent<Animator>(out var animator))
            {
                animator.SetTrigger(_initTrigger);
            }
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
                //currentStateProvider.SelectedIndex = viewWidgets.IndexOf(LastSelectedViewElement);
            }

            // 表示更新
            this.handler = handler;
            SetArg(manager, arg);
            UpdateElements(list);
            SetStatusCode(0);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            if (_scrollRect.vertical) { VerticalAbsolutePosition = local.VerticalAbsolutePosition; }
            if (_isSelectable) { local.ApplySelectedIndex(selectables); }
        }

        private void UpdateElements(IReadOnlyList<object> list)
        {
            // 前回生成したウィジェットをすべて削除
            foreach (var viewWidgetGameObject in viewWidgetGameObjects)
            {
                Destroy(viewWidgetGameObject);
            }
            viewElements.Clear();
            viewWidgets.Clear();
            selectables.Clear();
            viewWidgetGameObjects.Clear();

            // 今回必要なウィジェットを生成
            var sumHeight = 0f;
            var lastSumHeight = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                lastSumHeight = sumHeight;
                if (!ViewWidgetFactory.TryCreateViewWidget(this, handler, list[i], out var viewWidget))
                {
                    Debug.LogError($"{list[i]} の {nameof(ViewWidget)} を生成できません。");
                    continue;
                }

                viewWidget.SetParent(_scrollRect.content, false);
                viewWidget.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, sumHeight, viewWidget.rect.height);
                sumHeight += viewWidget.rect.height;

                viewElements.AddRange(viewWidget.GetComponentsInChildren<ViewElement>());
                viewWidgets.AddRange(viewWidget.GetComponentsInChildren<ViewWidget>());
                selectables.AddRange(viewWidget.GetComponentsInChildren<Selectable>());
                viewWidgetGameObjects.Add(viewWidget.gameObject);
            }

            var viewportHeight = _scrollRect.viewport.rect.height;
            var contentHeight = sumHeight;
            _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, contentHeight);

            marginHeight = contentHeight - viewportHeight;

            // スクロールしない場合は中央に寄せる
            if (!_scrollRect.vertical)
            {
                _scrollRect.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (viewportHeight - sumHeight) / 2f, contentHeight);
            }
        }

        public override void SetBlock(bool block)
        {
            base.SetBlock(block);
            foreach (var viewWidget in viewWidgets)
            {
                viewWidget.SetBlock(block);
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

            public void ApplySelectedIndex(List<Selectable> selectables)
            {
                if (SelectedIndex <= 0 || selectables.Count <= SelectedIndex || EventSystem.current == null)
                {
                    // 選択オブジェクトが見つからなければ最初の項目を選択
                    if (selectables.Count >= 1) { EventSystem.current.SetSelectedGameObject(selectables[0].gameObject); }
                    return;
                }

                EventSystem.current.SetSelectedGameObject(selectables[SelectedIndex].gameObject);
            }
        }
    }
}
