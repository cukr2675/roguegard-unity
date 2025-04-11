using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/Sub Views/LMF Carousel 3D Sub View")]
    public class Carousel3DSubView : ElementsSubView
    {
        [SerializeField] private Vector2 _axisAnchoredPosition = Vector2.zero;
        [SerializeField] private Quaternion _axisRotation = Quaternion.Euler(-15f, 0f, 0f);
        [SerializeField] private float _radius = 1f;

        [SerializeField] private RectTransform _content = null;
        [SerializeField] private ViewElement _prevButton = null;
        [SerializeField] private ViewElement _nextButton = null;

        [SerializeField] private ViewElement _viewElementPrefab = null;

        [Tooltip("指定のインデックスまで回転する速さ")]
        [SerializeField] private float _elasticity = 0.2f;

        private bool isInitialized;
        private float itemHeight;

        private IElementHandler handler;
        private readonly List<object> list = new();
        private readonly List<ViewElement> viewElements = new();
        private readonly List<RectTransform> zSortedViewElements = new();
        private StateProvider currentStateProvider;

        private int lastItemOffset;

        /// <summary>
        /// スクロールバーの遊び
        /// </summary>
        private float marginHeight;

        public int AngleIndex { get; private set; }

        public float AngleDegree { get; private set; }
        private float beforeAngleDegree;
        private const float angleDegreeEpsilon = 0.1f;

        private event HandleEndAnimation OnEndRotateAngle;

        public void Initialize()
        {
            LMFAssert.NotInitialized(this, isInitialized);
            itemHeight = _viewElementPrefab.GetComponent<RectTransform>().rect.height;
            beforeAngleDegree = -1f;
            isInitialized = true;

            if (_prevButton != null)
            {
                _prevButton.Initialize(this);
                _prevButton.SetElement(
                    SelectOption.Create<IListMenuManager, IListMenuArg>("", delegate { FocusAngleIndex(AngleIndex - 1); }), SelectOptionHandler.Instance);
                _prevButton.SetVisible(true, true);
            }
            if (_nextButton != null)
            {
                _nextButton.Initialize(this);
                _nextButton.SetElement(
                    SelectOption.Create<IListMenuManager, IListMenuArg>("", delegate { FocusAngleIndex(AngleIndex + 1); }), SelectOptionHandler.Instance);
                _nextButton.SetVisible(true, true);
            }
        }

        /// <summary>
        /// 指定のインデックスを 0 ~ 要素数 のループ空間に変換して設定する
        /// </summary>
        public void FocusAngleIndex(int index, HandleEndAnimation onEndRotateAngle = null)
        {
            AngleIndex = (index + viewElements.Count) % viewElements.Count;
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(viewElements[AngleIndex].gameObject);
                QueueSelect(gameObject, viewElements[AngleIndex].gameObject, CursorPlay.None);
            }
            OnEndRotateAngle += onEndRotateAngle;
        }

        private void LateUpdate()
        {
            if (viewElements.Count == 0) return;

            // カーソル移動で回転する
            var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
            if (selected != null && selected.TryGetComponent<ViewElement>(out var selectedViewElement))
            {
                var selectedElementIndex = viewElements.IndexOf(selectedViewElement);
                if (selectedElementIndex != -1) { AngleIndex = selectedElementIndex; }
            }

            // 目標の要素まで回転させる
            var targetAngle = 360f * AngleIndex / viewElements.Count;
            AngleDegree = Mathf.LerpAngle(AngleDegree, targetAngle, _elasticity);
            if (Mathf.Abs(AngleDegree - targetAngle) <= angleDegreeEpsilon)
            {
                // 回転終了
                AngleDegree = targetAngle;
                var onEndRotateAngle = OnEndRotateAngle;
                OnEndRotateAngle = null;
                onEndRotateAngle?.Invoke(Manager, Arg);
            }

            // アングルが変わっていなければ更新しない
            if (AngleDegree == beforeAngleDegree) return;
            beforeAngleDegree = AngleDegree;

            for (int i = 0; i < viewElements.Count; i++)
            {
                var viewElement = viewElements[i];
                var viewElementTransform = (RectTransform)viewElement.transform;
                viewElementTransform.anchoredPosition3D = GetCarouselAnchoredPosition((float)i / viewElements.Count);
            }

            zSortedViewElements.Sort((a, b) => -a.anchoredPosition3D.z.CompareTo(b.anchoredPosition3D.z));
            for (int i = zSortedViewElements.Count - 1; i >= 0; i--)
            {
                zSortedViewElements[i].SetSiblingIndex(i);
            }
        }

        /// <summary>
        /// 0 (始点) から 1 (終点 = 一周した始点) までの指定の場所の実座標を取得する
        /// </summary>
        private Vector3 GetCarouselAnchoredPosition(float t)
        {
            return (Vector3)_axisAnchoredPosition + (_axisRotation * Quaternion.Euler(0f, (AngleDegree - t * 360f), 0f) * (Vector3.back * _radius));
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
                currentStateProvider.AngleIndex = AngleIndex;
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
            InitElements();
            SetStatusCode(0);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            AngleIndex = local.AngleIndex;
            AngleDegree = 360f * AngleIndex / viewElements.Count;
            local.ApplySelectedIndex(this);
        }

        private void InitElements()
        {
            AdjustViewElements(list.Count);

            for (int i = 0; i < viewElements.Count; i++)
            {
                var viewElement = viewElements[i];
                viewElement.SetElement(list[i], handler);
            }
        }

        private void AdjustViewElements(int count)
        {
            if (viewElements.Count != count)
            {
                // 足りない ViewElement を生成する
                while (viewElements.Count < count)
                {
                    var viewElement = Instantiate(_viewElementPrefab, _content);
                    viewElement.Initialize(this);

                    viewElements.Add(viewElement);
                    ViewElement.SetHorizontalNavigation(viewElements, viewElements.Count - 1, loop: true); // ナビゲーションを明示したほうが長押し移動がスムーズになる？
                }

                // 不要な ViewElement は削除する
                if (viewElements.Count > count)
                {
                    for (int i = viewElements.Count - 1; i >= count; i--)
                    {
                        Destroy(viewElements[i].gameObject);
                    }
                    viewElements.RemoveRange(count, viewElements.Count - count);
                    ViewElement.SetHorizontalNavigation(viewElements, viewElements.Count - 1, loop: true);
                }

                zSortedViewElements.Clear();
                zSortedViewElements.AddRange(viewElements.Select(x => (RectTransform)x.transform));
            }
        }

        private class StateProvider : IElementsSubViewStateProvider
        {
            public int AngleIndex { get; set; }
            public int SelectedIndex { get; set; }

            public void Reset()
            {
                AngleIndex = 0;
                SelectedIndex = -1;
            }

            public void ApplySelectedIndex(Carousel3DSubView subView)
            {
                if (SelectedIndex <= 0 || subView.viewElements.Count <= SelectedIndex || EventSystem.current == null)
                {
                    // 選択オブジェクトが見つからなければ最初の項目を選択
                    if (ViewElement.TryFirstNotNull(subView.viewElements, out var first))
                    {
                        //EventSystem.current.SetSelectedGameObject(first.gameObject); // これだと Show メソッドで interactable が true になる前に選択してしまう
                        subView.QueueSelect(subView.gameObject, first.gameObject, CursorPlay.None);
                    }
                    return;
                }

                subView.QueueSelect(subView.gameObject, subView.viewElements[SelectedIndex].gameObject, CursorPlay.None);
            }
        }
    }
}
