using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class ScrollModelsMenuView : ModelsMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private ModelsMenuViewItemButton _sortButton = null;
        [SerializeField] private ModelsMenuViewItemButton _exitButton = null;
        [SerializeField] private ModelsMenuViewItemButton _itemPrefab = null;

        [Header("Layout")]
        [SerializeField] private float _itemWidth = 150f;

        private IModelsMenuItemController itemController;

        private readonly List<object> models = new List<object>();

        private List<ModelsMenuViewItemButton> itemButtons = new List<ModelsMenuViewItemButton>();

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private float itemHeight;
        private float scrollRectHeight;
        private float marginHeight;

        public void Initialize()
        {
            itemHeight = _itemPrefab.GetComponent<RectTransform>().rect.height;
            scrollRectHeight = _scrollRect.GetComponent<RectTransform>().rect.height;
            _sortButton.Initialize(this);
            _exitButton.Initialize(this);
            _scrollRect.onValueChanged.AddListener((x) => UpdateItems());
        }

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
            AdjustItems(7);
            var contentHeight = itemHeight * (models.Count - 1) + scrollRectHeight;
            marginHeight = contentHeight - scrollRectHeight;
            _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, contentHeight);
            var index = arg.Vector.x;
            //index = Mathf.Clamp(arg.Vector.x, 0f, Mathf.Floor(marginHeight / itemHeight));
            _scrollRect.verticalNormalizedPosition = 1f - (index * itemHeight / marginHeight);
            UpdateItems();
            MenuController.Show(_sortButton.CanvasGroup, false);
            MenuController.Show(_exitButton.CanvasGroup, false);
            MenuController.Show(_canvasGroup, true);
        }

        public override float GetPosition()
        {
            // 後からアイテムが増えたときのため、スクロール位置を変換したものを返す
            var offset = marginHeight * (1f - _scrollRect.verticalNormalizedPosition) / itemHeight;
            //offset = Mathf.Clamp(offset, 0f, Mathf.Floor(marginHeight / itemHeight));
            return offset;
        }

        public override void SetPosition(float position)
        {
            _scrollRect.verticalNormalizedPosition = position;
        }

        public void ShowExitButton(IModelsMenuChoice choice)
        {
            _exitButton.SetItem(ChoicesModelsMenuItemController.Instance, choice);
            MenuController.Show(_exitButton.CanvasGroup, true);
        }

        public void ShowSortButton(IModelsMenuChoice choice)
        {
            _sortButton.SetItem(ChoicesModelsMenuItemController.Instance, choice);
            MenuController.Show(_sortButton.CanvasGroup, true);
        }

        private void ShowSubmitButton(IModelsMenuChoice choice)
        {

        }

        private void UpdateItems()
        {
            var offset = Mathf.FloorToInt(marginHeight * (1f - _scrollRect.verticalNormalizedPosition) / itemHeight);
            for (int i = 0; i < itemButtons.Count; i++)
            {
                var itemButton = itemButtons[i];
                var itemButtonTransform = itemButton.RectTransform;
                var index = i + offset;
                if (index < 0 || models.Count <= index)
                {
                    MenuController.Show(itemButton.CanvasGroup, false);
                    continue;
                }

                itemButton.SetItem(itemController, models[index]);
                itemButtonTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, index * _itemWidth, _itemWidth);
                MenuController.Show(itemButton.CanvasGroup, true);
            }
        }

        private void AdjustItems(int count)
        {
            if (itemButtons.Count != count)
            {
                while (itemButtons.Count < count)
                {
                    var itemButton = Instantiate(_itemPrefab, _scrollRect.content.transform);
                    itemButton.Initialize(this);
                    var itemButtonTransform = itemButton.RectTransform;
                    itemButtonTransform.anchorMin = new Vector2(0f, 0f);
                    itemButtonTransform.anchorMax = new Vector2(1f, 0f);
                    itemButtonTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, _scrollRect.content.rect.width);

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
            }
        }
    }
}
