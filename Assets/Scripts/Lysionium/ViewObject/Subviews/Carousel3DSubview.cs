using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/Subviews/LUI Carousel 3D Subview")]
    public class Carousel3DSubview : ListHandlerSubview
    {
        [SerializeField] private Vector2 _axisAnchoredPosition = Vector2.zero;
        [SerializeField] private Quaternion _axisRotation = Quaternion.Euler(-15f, 0f, 0f);
        [SerializeField] private float _radius = 1f;

        [SerializeField] private RectTransform _content = null;
        [SerializeField] private ViewItem _prevButton = null;
        [SerializeField] private ViewItem _nextButton = null;

        [SerializeField] private ViewItem _viewItemPrefab = null;

        [Tooltip("指定のインデックスまで回転する速さ")]
        [SerializeField] private float _elasticity = 0.2f;

        private float itemHeight;

        private IViewItemHandler handler;
        private readonly List<object> list = new();
        private readonly List<ViewItem> viewItems = new();
        private readonly List<RectTransform> zSortedViewItems = new();
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

        private event ListuiEventHandler OnEndRotateAngle;

        protected override void CommonInitCore()
        {
            itemHeight = _viewItemPrefab.GetComponent<RectTransform>().rect.height;
            beforeAngleDegree = -1f;
        }

        /// <summary>
        /// 指定のインデックスを 0 ~ 要素数 のループ空間に変換して設定する
        /// </summary>
        public void FocusAngleIndex(int index, ListuiEventHandler onEndRotateAngle = null)
        {
            AngleIndex = (index + viewItems.Count) % viewItems.Count;
            if (EventSystem != null)
            {
                EventSystem.SetSelectedGameObject(viewItems[AngleIndex].gameObject);
                QueueSelect(gameObject, viewItems[AngleIndex].gameObject, CursorPlay.None);
            }
            OnEndRotateAngle += onEndRotateAngle;
        }

        protected virtual void LateUpdate()
        {
            if (viewItems.Count == 0) return;

            // カーソル移動で回転する
            var selected = EventSystem != null ? EventSystem.currentSelectedGameObject : null;
            if (selected != null && selected.TryGetComponent<ViewItem>(out var selectedViewItem))
            {
                var selectedItemIndex = viewItems.IndexOf(selectedViewItem);
                if (selectedItemIndex != -1) { AngleIndex = selectedItemIndex; }
            }

            // 目標の要素まで回転させる
            var targetAngle = 360f * AngleIndex / viewItems.Count;
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

            for (int i = 0; i < viewItems.Count; i++)
            {
                var viewItem = viewItems[i];
                var viewItemTransform = (RectTransform)viewItem.transform;
                viewItemTransform.anchoredPosition3D = GetCarouselAnchoredPosition((float)i / viewItems.Count);
            }

            zSortedViewItems.Sort((a, b) => -a.anchoredPosition3D.z.CompareTo(b.anchoredPosition3D.z));
            for (int i = zSortedViewItems.Count - 1; i >= 0; i--)
            {
                zSortedViewItems[i].SetSiblingIndex(i);
            }
        }

        /// <summary>
        /// 0 (始点) から 1 (終点 = 一周した始点) までの指定の場所の実座標を取得する
        /// </summary>
        private Vector3 GetCarouselAnchoredPosition(float t)
        {
            return (Vector3)_axisAnchoredPosition + (_axisRotation * Quaternion.Euler(0f, (AngleDegree - t * 360f), 0f) * (Vector3.back * _radius));
        }

        public override void SetListHandler(
            IReadOnlyList<object> list, IViewItemHandler handler, IListuiManager manager, IListuiArg arg,
            ref ISubviewStateProvider stateProvider)
        {
            if (stateProvider == null) { stateProvider = new StateProvider(); }
            if (!(stateProvider is StateProvider local)) throw new System.ArgumentException(
                $"{stateProvider} は {nameof(StateProvider)} ではありません。");

            // 現在の StateProvider を外す前に状態を保存する
            if (currentStateProvider != null)
            {
                currentStateProvider.AngleIndex = AngleIndex;
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

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            AngleIndex = local.AngleIndex;
            AngleDegree = 360f * AngleIndex / viewItems.Count;
            local.ApplySelectedIndex(this);
        }

        private void InitViewItems()
        {
            AdjustViewItems(list.Count);

            for (int i = 0; i < viewItems.Count; i++)
            {
                var viewItem = viewItems[i];
                viewItem.Bind(list[i], handler);
            }

            if (_prevButton != null)
            {
                _prevButton.Initialize(this);
                _prevButton.Bind(new SelectOption("", delegate { FocusAngleIndex(AngleIndex - 1); }));
                _prevButton.SetVisible(true, true);
            }
            if (_nextButton != null)
            {
                _nextButton.Initialize(this);
                _nextButton.Bind(new SelectOption("", delegate { FocusAngleIndex(AngleIndex + 1); }));
                _nextButton.SetVisible(true, true);
            }
        }

        private void AdjustViewItems(int count)
        {
            if (viewItems.Count != count)
            {
                // 足りない ViewItem を生成する
                while (viewItems.Count < count)
                {
                    var viewItem = Instantiate(_viewItemPrefab, _content);
                    viewItem.Initialize(this);

                    viewItems.Add(viewItem);
                    ViewItem.SetHorizontalNavigation(viewItems, viewItems.Count - 1, loop: true); // ナビゲーションを明示したほうが長押し移動がスムーズになる？
                }

                // 不要な ViewItem は削除する
                if (viewItems.Count > count)
                {
                    for (int i = viewItems.Count - 1; i >= count; i--)
                    {
                        Destroy(viewItems[i].gameObject);
                    }
                    viewItems.RemoveRange(count, viewItems.Count - count);
                    ViewItem.SetHorizontalNavigation(viewItems, viewItems.Count - 1, loop: true);
                }

                zSortedViewItems.Clear();
                zSortedViewItems.AddRange(viewItems.Select(x => (RectTransform)x.transform));
            }
        }

        private class StateProvider : ISubviewStateProvider
        {
            public int AngleIndex { get; set; }
            public int SelectedIndex { get; set; }

            public void Reset()
            {
                AngleIndex = 0;
                SelectedIndex = -1;
            }

            public void ApplySelectedIndex(Carousel3DSubview subview)
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
