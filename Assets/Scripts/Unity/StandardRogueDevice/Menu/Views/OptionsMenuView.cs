using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using RuntimeDotter;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class OptionsMenuView : ModelsMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private DotterColorPicker _colorPicker = null;
        [Space]
        [SerializeField] private ModelsMenuViewItemButton _choicePrefab = null;
        [SerializeField] private ModelsMenuViewOptionSlider _sliderPrefab = null;
        [SerializeField] private ModelsMenuViewOptionText _textPrefab = null;
        [SerializeField] private ModelsMenuViewOptionCheckBox _checkBoxPrefab = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private readonly List<GameObject> items = new List<GameObject>();

        private ColorItemController colorItemController;

        public void Initialize()
        {
            colorItemController = new ColorItemController() { parent = this };
            _colorPicker.OnColorChanged.AddListener(x => colorItemController.OnColorChanged(Root, Self, User, Arg, x.ToPickerColor()));
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models,
            IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(root, self, user, arg);
            foreach (var item in items)
            {
                Destroy(item);
            }
            items.Clear();

            var sumHeight = 0f;
            for (int i = 0; i < models.Count; i++)
            {
                var model = models[i];
                if (model is IModelsMenuOptionSlider slider)
                {
                    var item = Instantiate(_sliderPrefab, _scrollRect.content);
                    SetTransform((RectTransform)item.transform, ref sumHeight);

                    var label = slider.GetName(Root, Self, User, Arg);
                    var value = slider.GetValue(Root, Self, User, Arg);
                    item.Initialize(label, value, value => slider.SetValue(Root, Self, User, Arg, value));
                    items.Add(item.gameObject);
                }
                else if (model is IModelsMenuOptionText text)
                {
                    var item = Instantiate(_textPrefab, _scrollRect.content);
                    SetTransform((RectTransform)item.transform, ref sumHeight);

                    var label = text.GetName(Root, Self, User, Arg);
                    var value = text.GetValue(Root, Self, User, Arg);
                    item.Initialize(label, value, value => text.SetValue(Root, Self, User, Arg, value));
                    items.Add(item.gameObject);
                }
                else if (model is IModelsMenuOptionCheckBox checkBox)
                {
                    var item = Instantiate(_checkBoxPrefab, _scrollRect.content);
                    SetTransform((RectTransform)item.transform, ref sumHeight);

                    var label = checkBox.GetName(Root, Self, User, Arg);
                    var value = checkBox.GetValue(Root, Self, User, Arg);
                    item.Initialize(label, value, value => checkBox.SetValue(Root, Self, User, Arg, value));
                    items.Add(item.gameObject);
                }
                else if (model is IModelsMenuOptionColor color)
                {
                    var item = Instantiate(_choicePrefab, _scrollRect.content);
                    SetTransform((RectTransform)item.transform, ref sumHeight);

                    item.Initialize(this);
                    item.SetItem(colorItemController, color);
                    items.Add(item.gameObject);
                }
                else
                {
                    var item = Instantiate(_choicePrefab, _scrollRect.content);
                    SetTransform((RectTransform)item.transform, ref sumHeight);

                    item.Initialize(this);
                    item.SetItem(itemController, model);
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

        private class ColorItemController : IModelsMenuItemController
        {
            public OptionsMenuView parent;
            private IModelsMenuOptionColor currentColor;

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var color = (IModelsMenuOptionColor)model;
                return color.GetName(root, self, user, arg);
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                currentColor = (IModelsMenuOptionColor)model;
                var value = currentColor.GetValue(root, self, user, arg);
                parent._colorPicker.Open(new ShiftableColor(value, false), true);
            }

            public void OnColorChanged(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, Color value)
            {
                currentColor.SetValue(root, self, user, arg, value);
            }
        }
    }
}
