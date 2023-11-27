using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class GridModelsMenuView : ModelsMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private RectTransform _content = null;
        [SerializeField] private ModelsMenuViewItemButton _itemPrefab = null;

        [Header("Layout")]
        [SerializeField] private Vector2Int _itemButtonCount = Vector2Int.one;
        [SerializeField] private RectTransform _verticalExtensionTarget = null;

        private IModelsMenuItemController itemController;

        private readonly List<object> models = new List<object>();

        private List<ModelsMenuViewItemButton> itemButtons = new List<ModelsMenuViewItemButton>();

        public override CanvasGroup CanvasGroup => _canvasGroup;

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models,
            IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            this.itemController = itemController;
            this.models.Clear();
            for (int i = 0; i < models.Count; i++)
            {
                this.models.Add(models[i]);
            }
            SetArg(root, self, user, arg);
            UpdateItems();
            MenuController.Show(_canvasGroup, true);
        }

        public override float GetPosition() => 0f;
        public override void SetPosition(float position) { }

        private void UpdateItems()
        {
            var length = _itemButtonCount.x * _itemButtonCount.y;
            length = Mathf.Min(length, models.Count);
            if (_verticalExtensionTarget != null) { length = models.Count; }
            AdjustItems(length);
            for (int i = 0; i < itemButtons.Count; i++)
            {
                var itemButton = itemButtons[i];
                itemButton.SetItem(itemController, models[i]);
            }
        }

        private void AdjustItems(int count)
        {
            if (itemButtons.Count != count)
            {
                var contentTransform = _content.transform;
                var rect = ((RectTransform)contentTransform).rect;
                var itemSize = rect.size / _itemButtonCount;

                while (itemButtons.Count < count)
                {
                    var index = itemButtons.Count;
                    var x = index % _itemButtonCount.x;
                    var y = index / _itemButtonCount.x;
                    if (y > _itemButtonCount.y) throw new RogueException();

                    var itemButton = Instantiate(_itemPrefab, _content.transform);
                    itemButton.Initialize(this);
                    var itemButtonTransform = (RectTransform)itemButton.transform;
                    itemButtonTransform.anchorMin = new Vector2(0f, 0f);
                    itemButtonTransform.anchorMax = new Vector2(1f, 0f);
                    itemButtonTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x * itemSize.x, itemSize.x);
                    itemButtonTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y * itemSize.y, itemSize.y);

                    itemButtons.Add(itemButton);
                }
                if (itemButtons.Count > count)
                {
                    for (int i = count; i < itemButtons.Count; i++)
                    {
                        Destroy(itemButtons[i].gameObject);
                    }
                    itemButtons.RemoveRange(count, itemButtons.Count - count);
                }

                // 選択肢の要素数に合わせてウィンドウを拡張する
                if (_verticalExtensionTarget != null)
                {
                    var height = itemSize.y * count;
                    var sizeDelta = _verticalExtensionTarget.sizeDelta;
                    sizeDelta.y = sizeDelta.y - _content.rect.height + height;
                    _verticalExtensionTarget.sizeDelta = sizeDelta;
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
            var itemSize = size / _itemButtonCount;
            Gizmos.color = Color.cyan;
            for (int y = 0; y < _itemButtonCount.y; y++)
            {
                for (int x = 0; x < _itemButtonCount.x; x++)
                {
                    var position = startPosition + itemSize * new Vector2(x + 0.5f, y + 0.5f);
                    Gizmos.DrawWireCube(position, itemSize);
                }
            }
        }
#endif

        #endregion
    }
}
