using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using RuntimeDotter;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class OptionsMenuView : ElementsView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private DotterColorPicker _colorPicker = null;
        [Space]
        [SerializeField] private ViewElementButton _selectOptionPrefab = null;
        [SerializeField] private OptionsViewSlider _sliderPrefab = null;
        [SerializeField] private OptionsViewText _textPrefab = null;
        [SerializeField] private OptionsViewCheckBox _checkBoxPrefab = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private readonly List<GameObject> items = new List<GameObject>();

        private ColorPresenter colorPresenter;

        public void Initialize()
        {
            colorPresenter = new ColorPresenter() { parent = this };
            _colorPicker.OnColorChanged.AddListener(x => colorPresenter.OnColorChanged(Root, Self, User, Arg, x.ToPickerColor()));
        }

        public override void OpenView<T>(
            IElementPresenter presenter, Spanning<T> list, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(manager, self, user, arg);
            foreach (var item in items)
            {
                Destroy(item);
            }
            items.Clear();

            var sumHeight = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                var element = list[i];
                if (element is IOptionsMenuSlider slider)
                {
                    var item = Instantiate(_sliderPrefab, _scrollRect.content);
                    SetTransform((RectTransform)item.transform, ref sumHeight);

                    var label = slider.GetName(Root, Self, User, Arg);
                    var value = slider.GetValue(Root, Self, User, Arg);
                    item.Initialize(label, value, value => slider.SetValue(Root, Self, User, Arg, value));
                    items.Add(item.gameObject);
                }
                else if (element is IOptionsMenuText text)
                {
                    var item = Instantiate(_textPrefab, _scrollRect.content);
                    SetTransform((RectTransform)item.transform, ref sumHeight);

                    var label = text.GetName(Root, Self, User, Arg);
                    var value = text.GetValue(Root, Self, User, Arg);
                    item.Initialize(label, value, value => text.SetValue(Root, Self, User, Arg, value));
                    items.Add(item.gameObject);
                }
                else if (element is IOptionsMenuCheckBox checkBox)
                {
                    var item = Instantiate(_checkBoxPrefab, _scrollRect.content);
                    SetTransform((RectTransform)item.transform, ref sumHeight);

                    var label = checkBox.GetName(Root, Self, User, Arg);
                    var value = checkBox.GetValue(Root, Self, User, Arg);
                    item.Initialize(label, value, value => checkBox.SetValue(Root, Self, User, Arg, value));
                    items.Add(item.gameObject);
                }
                else if (element is IOptionsMenuColor color)
                {
                    var item = Instantiate(_selectOptionPrefab, _scrollRect.content);
                    SetTransform((RectTransform)item.transform, ref sumHeight);

                    item.Initialize(this);
                    item.SetItem(colorPresenter, color);
                    items.Add(item.gameObject);
                }
                else
                {
                    var item = Instantiate(_selectOptionPrefab, _scrollRect.content);
                    SetTransform((RectTransform)item.transform, ref sumHeight);

                    item.Initialize(this);
                    item.SetItem(presenter, element);
                    items.Add(item.gameObject);
                }
            }
            _scrollRect.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, sumHeight);
            MenuController.Show(_canvasGroup, true);
        }

        private static void SetTransform(RectTransform itemTransform, ref float sumHeight)
        {
            itemTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, sumHeight, itemTransform.rect.height);
            sumHeight += itemTransform.rect.height;
        }

        public override float GetPosition()
        {
            // 後からアイテムが増えたときのため、スクロール位置を変換したものを返す
            var offset = _scrollRect.content.rect.height * (1f - _scrollRect.verticalNormalizedPosition);
            return offset;
        }

        public override void SetPosition(float position)
        {
            _scrollRect.verticalNormalizedPosition = position;
        }

        private class ColorPresenter : IElementPresenter
        {
            public OptionsMenuView parent;
            private IOptionsMenuColor currentColor;

            public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var color = (IOptionsMenuColor)element;
                return color.GetName(manager, self, user, arg);
            }

            public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                currentColor = (IOptionsMenuColor)element;
                var value = currentColor.GetValue(manager, self, user, arg);
                parent._colorPicker.Open(new ShiftableColor(value, false), true);
            }

            public void OnColorChanged(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, Color value)
            {
                currentColor.SetValue(manager, self, user, arg, value);
            }
        }
    }
}
