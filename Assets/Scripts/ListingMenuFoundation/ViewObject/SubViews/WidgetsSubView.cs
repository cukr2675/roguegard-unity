using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.UI;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/Sub Views/LMF Widgets Sub View")]
    public class WidgetsSubView : ElementsSubView
    {
        [SerializeField] private ScrollRect _scrollRect = null;

        // シーン遷移でフェードインさせるとき1フレーム目から表示させるために必要。ほかの SubView では使わないのでこのクラスのみ実装
        [Tooltip("Awake で起動するトリガー名\nこの SubView を表示状態で生成したいときに使う")]
        [SerializeField] private string _initTrigger = null;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = null;
        private Animator animator;

        [Header("Other")]

        [Tooltip("この値が true のときカーソル移動の対象となる")]
        [SerializeField] private bool _isSelectable = true;

        private IElementHandler handler;
        private readonly List<ViewElement> viewElements = new();
        private readonly List<ViewWidget> viewWidgets = new();
        private Selectable fallbackSelectable;
        private readonly List<GameObject> viewWidgetRootObjs = new();
        private StateProvider currentStateProvider;
        private string style;

        /// <summary>
        /// スクロールバーの遊び
        /// </summary>
        private Vector2 marginSize;

        /// <summary>
        /// <see cref="ScrollRect.content"/> の幅が固定されているか
        /// </summary>
        private bool fixedContentWidth;

        private float VerticalAbsolutePosition
        {
            get
            {
                // 後から要素が増えたときのため、スクロール位置を変換したものを返す
                return (1f - _scrollRect.verticalNormalizedPosition) * marginSize.y;
            }
            set
            {
                const float epsilon = 1e-4f;
                if (marginSize.y < epsilon) { _scrollRect.verticalNormalizedPosition = 0f; } // ゼロ除算対策
                else { _scrollRect.verticalNormalizedPosition = 1f - (value / marginSize.y); }
            }
        }

        private float HorizontalAbsolutePosition
        {
            get
            {
                // 後から要素が増えたときのため、スクロール位置を変換したものを返す
                return _scrollRect.horizontalNormalizedPosition * marginSize.x;
            }
            set
            {
                const float epsilon = 1e-4f;
                if (marginSize.x < epsilon) { _scrollRect.horizontalNormalizedPosition = 0f; } // ゼロ除算対策
                else { _scrollRect.horizontalNormalizedPosition = value / marginSize.x; }
            }
        }

        private void Awake()
        {
            if (TryGetComponent(out animator))
            {
                if (!string.IsNullOrWhiteSpace(_defaultStyle)) { SetStyle(_defaultStyle); }
                if (!string.IsNullOrWhiteSpace(_initTrigger) && TryGetComponent(out animator)) { animator.SetTrigger(_initTrigger); }
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
                currentStateProvider.HorizontalAbsolutePosition = HorizontalAbsolutePosition;
                currentStateProvider.LoadSelectedElementName(LastSelectedObj);
            }

            // 表示更新
            this.handler = handler;
            SetArg(manager, arg);
            UpdateElements(list);
            SetStatusCode(0);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            if (_scrollRect.vertical) { VerticalAbsolutePosition = local.VerticalAbsolutePosition; }
            HorizontalAbsolutePosition = local.HorizontalAbsolutePosition;
            if (_isSelectable) { local.ApplySelectedIndex(this); }
        }

        private void UpdateElements(IReadOnlyList<object> list)
        {
            // ビューの状態を初期化
            _scrollRect.horizontal = false;
            fixedContentWidth = false;

            // 前回のスタイルを解除する
            SetStyle(null);

            var viewportSize = _scrollRect.viewport.rect.size;
            var contentSize = new Vector2(viewportSize.x, 0f);

            _scrollRect.content.anchorMin = new Vector2(0f, 1f);
            _scrollRect.content.anchorMax = new Vector2(0f, 1f);
            _scrollRect.content.sizeDelta = contentSize;
            marginSize = contentSize - viewportSize;

            // 前回生成したウィジェットをすべて削除
            foreach (var viewWidgetRootObj in viewWidgetRootObjs)
            {
                Destroy(viewWidgetRootObj);
            }
            viewElements.Clear();
            viewWidgets.Clear();
            fallbackSelectable = null;
            viewWidgetRootObjs.Clear();

            // 今回必要なウィジェットを生成
            var sumHeight = 0f;
            var maxWidth = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                if (!ViewWidgetFactory.TryCreateViewWidget(list[i], handler, this, out var viewWidget))
                {
                    Debug.LogError($"{list[i]} の {nameof(ViewWidget)} を生成できません。");
                    continue;
                }

                viewWidget.SetParent(_scrollRect.content, false);
                viewWidget.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, sumHeight, viewWidget.rect.height);
                sumHeight += viewWidget.rect.height;
                maxWidth = Mathf.Max(maxWidth, viewWidget.rect.width);

                viewElements.AddRange(viewWidget.GetComponentsInChildren<ViewElement>());
                viewWidgets.AddRange(viewWidget.GetComponentsInChildren<ViewWidget>());
                if (fallbackSelectable == null) { fallbackSelectable = viewWidget.GetComponentInChildren<Selectable>(); }
                viewWidgetRootObjs.Add(viewWidget.gameObject);
            }

            // スタイルが設定されていなければデフォルトを使用
            if (style == null && !string.IsNullOrWhiteSpace(_defaultStyle)) { SetStyle(_defaultStyle); }

            contentSize.y = sumHeight;
            if (fixedContentWidth) { contentSize.x = _scrollRect.content.sizeDelta.x; }

            _scrollRect.content.anchorMin = new Vector2(0f, 1f);
            _scrollRect.content.anchorMax = new Vector2(0f, 1f);
            _scrollRect.content.sizeDelta = contentSize;
            marginSize = contentSize - viewportSize;

            // スクロールしない場合は中央に寄せる
            if (!_scrollRect.vertical)
            {
                _scrollRect.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (viewportSize.y - sumHeight) / 2f, contentSize.y);
            }
        }

        // HeaderViewWidget から呼び出す用
        public void SetContentWidth(float contentWidth)
        {
            var viewportWidth = _scrollRect.viewport.rect.width;

            _scrollRect.horizontal = true;
            _scrollRect.content.anchorMin = new Vector2(0f, 1f);
            _scrollRect.content.anchorMax = new Vector2(0f, 1f);
            _scrollRect.content.sizeDelta = new Vector2(contentWidth, _scrollRect.content.sizeDelta.y);
            marginSize.x = contentWidth - viewportWidth;
            fixedContentWidth = true;
        }

        public void SetStyle(string newStyle)
        {
            if (newStyle == style) return;

            // この値が true のとき新しいスタイルの適用、 false のとき設定済みスタイルの初期化
            var apply = newStyle != null;

            if (apply && style != null) throw new InvalidOperationException($"スタイル ({style}) 解除前に新しいスタイルを適用することはできません。");

            // 新しいスタイルを保持
            if (apply) { style = newStyle; }

            // スタイルをスペース区切りで処理する
            for (int i = 0; i < style.Length; i++)
            {
                if ((i == 0 || style[i - 1] == ' ') && style[i] != ' ')
                {
                    var styleItemStart = i;
                    var styleItemLength = style.IndexOf(' ', styleItemStart);
                    if (styleItemLength == -1) { styleItemLength = style.Length - styleItemStart; }
                    i = styleItemStart + styleItemLength;

                    // スペース区切りで取得したスタイル名
                    var styleItem = style.AsSpan(styleItemStart, styleItemLength);

                    // AnimationController のレイヤーの重みをスタイル名で変更する
                    if (!styleItem.Contains(":".AsSpan(), StringComparison.CurrentCulture) && animator != null)
                    {
                        var any = false;
                        for (int j = 0; j < animator.layerCount; j++)
                        {
                            if (EqualsIgnoreWhiteSpace(animator.GetLayerName(j), styleItem))
                            {
                                // スタイル名と一致するレイヤーの重みを更新する
                                var weight = apply ? 1f : 0f;
                                animator.SetLayerWeight(j, weight);
                                any = true;
                            }
                        }

                        if (apply && !any)
                        {
                            // レイヤーが見つからなければ警告
                            Debug.LogWarning($"レイヤー {new string(styleItem)} が見つかりませんでした。存在するレイヤー: {string.Join(", ", GetLayerNames(animator))}");
                        }
                    }
                }
            }

            // 設定済みスタイルを破棄
            if (!apply) { style = null; }
        }

        private static bool EqualsIgnoreWhiteSpace(string layerName, ReadOnlySpan<char> style)
        {
            var styleIndex = 0;
            for (int i = 0; i < layerName.Length; i++)
            {
                if (layerName[i] == ' ') continue; // レイヤー名の空白はないものとして判定する

                if (layerName[i] != style[styleIndex]) return false;

                styleIndex++;
            }
            return true;
        }

        private static IEnumerable<string> GetLayerNames(Animator animator)
        {
            for (int i = 0; i < animator.layerCount; i++)
            {
                yield return animator.GetLayerName(i);
            }
        }

        private class StateProvider : IElementsSubViewStateProvider
        {
            public float VerticalAbsolutePosition { get; set; }
            public float HorizontalAbsolutePosition { get; set; }
            public int SelectedIndex { get; set; }
            public string SelectedName { get; set; }
            public bool SelectedIsWidget { get; set; }

            public void Reset()
            {
                VerticalAbsolutePosition = 0f;
                HorizontalAbsolutePosition = 0f;
                SelectedIndex = -1;
            }

            public void LoadSelectedElementName(GameObject lastSelectedObj)
            {
                if (lastSelectedObj == null)
                {
                    SelectedName = null;
                }
                else if (lastSelectedObj.TryGetComponent<ViewWidget>(out var lastSelectedViewWidget))
                {
                    SelectedName = lastSelectedViewWidget.WidgetName;
                    SelectedIsWidget = true;
                }
                else if (lastSelectedObj.TryGetComponent<ViewElement>(out var lastSelectedViewElement))
                {
                    SelectedName = lastSelectedViewElement.ElementName;
                    SelectedIsWidget = false;
                }
                else
                {
                    SelectedName = null;
                }
            }

            public void ApplySelectedIndex(WidgetsSubView subView)
            {
                if (SelectedName != null)
                {
                    if (SelectedIsWidget)
                    {
                        foreach (var widget in subView.viewWidgets)
                        {
                            if (widget.WidgetName == SelectedName)
                            {
                                //EventSystem.current.SetSelectedGameObject(widget.gameObject); // これだと Show メソッドで interactable が true になる前に選択してしまう
                                subView.QueueSelect(subView.gameObject, widget.gameObject, CursorPlay.None);
                                return;
                            }
                        }
                    }
                    else
                    {
                        foreach (var element in subView.viewElements)
                        {
                            if (element.ElementName == SelectedName)
                            {
                                //EventSystem.current.SetSelectedGameObject(element.gameObject); // これだと Show メソッドで interactable が true になる前に選択してしまう
                                subView.QueueSelect(subView.gameObject, element.gameObject, CursorPlay.None);
                                return;
                            }
                        }
                    }
                }

                // 選択オブジェクトが見つからなければ最初の項目を選択
                if (subView.fallbackSelectable != null) { subView.QueueSelect(subView.gameObject, subView.fallbackSelectable.gameObject, CursorPlay.None); }
            }
        }
    }
}
