using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class OptionsMenuView : ModelsMenuView, IScrollModelsMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private ModelsMenuViewItemButton _exitButton = null;
        [Space]
        [SerializeField] private ModelsMenuViewItemButton _choicePrefab = null;
        [SerializeField] private ModelsMenuViewOptionSlider _sliderPrefab = null;
        [SerializeField] private ModelsMenuViewOptionText _textPrefab = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private readonly List<GameObject> items = new List<GameObject>();

        public void Initialize()
        {
            _exitButton.Initialize(this);
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

        public void ShowExitButton(IModelsMenuChoice choice)
        {
            _exitButton.SetItem(ChoicesModelsMenuItemController.Instance, choice);
            MenuController.Show(_exitButton.CanvasGroup, true);
        }
    }
}
