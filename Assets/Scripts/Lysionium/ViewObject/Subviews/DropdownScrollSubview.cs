using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/Subviews/LUI Dropdown Scroll Subview")]
    public class DropdownScrollSubview : ListHandlerSubview
    {
        [SerializeField] private GridViewContainer _viewContainerPrefab = null;

        [Header("Layout")]

        [Tooltip("グリッド分割数")]
        [SerializeField] private Vector2Int _viewItemCount = Vector2Int.one;

        [Tooltip("拡張方向（はい/いいえ等の選択肢用）")]
        [SerializeField] private ExtensionDirection _extensionDirection = ExtensionDirection.NotExtend;

        [Header("Other")]

        [Tooltip("この値が true のときカーソル移動の対象となる")]
        [SerializeField] private bool _isSelectable = true;

        private IViewItemHandler handler;
        private readonly List<GridViewContainer> viewContainers = new();
        private StateProvider currentStateProvider;

        public override void SetListHandler(
            IReadOnlyList<object> list, IViewItemHandler handler, IListuiManager manager, ref ISubviewStateProvider stateProvider)
        {
            stateProvider ??= new StateProvider();
            if (stateProvider is not StateProvider local) throw new System.ArgumentException(
                $"{stateProvider} は {nameof(StateProvider)} ではありません。");

            // 現在の StateProvider を外す前に状態を保存する
            if (currentStateProvider != null)
            {
            }

            this.handler = handler;
            Manager = manager;
            UpdateViewItems(list);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            if (_isSelectable) { local.ApplySelectedIndex(this); }
        }

        private void UpdateViewItems(IReadOnlyList<object> list)
        {
            var viewContainer = Instantiate(_viewContainerPrefab, transform);
            viewContainer.SetListHandler(list, handler, this, ((RectTransform)transform).rect);
            viewContainers.Add(viewContainer);
        }

        public void SetPosition(Rect rect)
        {
            var rectTransform = (RectTransform)transform;
            rectTransform.anchoredPosition = rect.max;
        }

        private enum ExtensionDirection
        {
            NotExtend,
            Up,
        }

        private class StateProvider : ISubviewStateProvider
        {
            //public List<int> TreeSelectedIndex { get; set; }

            public void Reset()
            {
                //TreeSelectedIndex.Clear();
            }

            public void ApplySelectedIndex(DropdownScrollSubview subview)
            {
                // 選択オブジェクトが見つからなければ最初の項目を選択
                if (ViewItem.TryFirstNotNull(subview.viewContainers[0].ViewItems, out var first))
                {
                    //subview.EventSystem.SetSelectedGameObject(first.gameObject); // これだと Show メソッドで interactable が true になる前に選択してしまう
                    subview.QueueSelect(subview.gameObject, first.gameObject, CursorEvtfx.None);
                }
            }
        }
    }
}
