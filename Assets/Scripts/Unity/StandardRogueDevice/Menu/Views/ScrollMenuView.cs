using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class ScrollMenuView : ElementsView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private ViewElementButton _itemPrefab = null;

        [Header("Layout")]
        [SerializeField] private float _itemWidth = 150f;

        //private IElementPresenter presenter;
        //private readonly List<object> list = new();
        //private readonly List<ViewElementButton> itemButtons = new();

        //public override CanvasGroup CanvasGroup => _canvasGroup;

        //private float itemHeight;
        //private RectTransform scrollTransform;
        //private float marginHeight;

        //public void Initialize()
        //{
        //    itemHeight = _itemPrefab.GetComponent<RectTransform>().rect.height;
        //    scrollTransform = _scrollRect.GetComponent<RectTransform>(); // 画面サイズの変更に対応するため Transform を記憶する
        //    _scrollRect.onValueChanged.AddListener((x) => UpdateItems());
        //}

        //public override void OpenView<T>(
        //    IElementPresenter presenter, Spanning<T> list, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //{
        //    this.presenter = presenter;
        //    this.list.Clear();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        this.list.Add(list[i]);
        //    }
        //    SetArg(manager, self, user, arg);
        //    UpdateItems();
        //    MenuController.Show(_canvasGroup, true);
        //}

        //public override float GetPosition()
        //{
        //    // 後からアイテムが増えたときのため、スクロール位置を変換したものを返す
        //    var offset = marginHeight * (1f - _scrollRect.verticalNormalizedPosition) / itemHeight;
        //    return offset;
        //}

        //public override void SetPosition(float position)
        //{
        //    _scrollRect.verticalNormalizedPosition = 1f - (position * itemHeight / marginHeight);
        //    UpdateItems();
        //}

        //private void UpdateItems()
        //{
        //    var scrollRectHeight = scrollTransform.rect.height;
        //    AdjustItems(Mathf.CeilToInt(scrollRectHeight / itemHeight) + 1);
        //    var contentHeight = itemHeight * (list.Count - 1) + scrollRectHeight;
        //    marginHeight = contentHeight - scrollRectHeight;
        //    _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, contentHeight);

        //    var offset = Mathf.FloorToInt(marginHeight * (1f - _scrollRect.verticalNormalizedPosition) / itemHeight);
        //    for (int i = 0; i < itemButtons.Count; i++)
        //    {
        //        var itemButton = itemButtons[i];
        //        var itemButtonTransform = itemButton.RectTransform;
        //        var index = i + offset;
        //        if (index < 0 || list.Count <= index)
        //        {
        //            MenuController.Show(itemButton.CanvasGroup, false);
        //            continue;
        //        }

        //        itemButton.SetItem(presenter, list[index]);
        //        itemButtonTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, index * _itemWidth, _itemWidth);
        //        MenuController.Show(itemButton.CanvasGroup, true);
        //    }
        //}

        //private void AdjustItems(int count)
        //{
        //    if (itemButtons.Count != count)
        //    {
        //        while (itemButtons.Count < count)
        //        {
        //            var itemButton = Instantiate(_itemPrefab, _scrollRect.content.transform);
        //            itemButton.Initialize(this);
        //            var itemButtonTransform = itemButton.RectTransform;
        //            itemButtonTransform.anchorMin = new Vector2(0f, 0f);
        //            itemButtonTransform.anchorMax = new Vector2(1f, 0f);
        //            itemButtonTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, _scrollRect.content.rect.width);

        //            itemButtons.Add(itemButton);
        //        }
        //        if (itemButtons.Count > count)
        //        {
        //            for (int i = count; i < itemButtons.Count; i++)
        //            {
        //                Destroy(itemButtons[i].gameObject);
        //            }
        //            itemButtons.RemoveRange(count, itemButtons.Count - count);
        //        }
        //    }
        //}
    }
}
