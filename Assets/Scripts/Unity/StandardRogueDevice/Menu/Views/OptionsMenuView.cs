using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;
using TMPro;

namespace RoguegardUnity
{
    public class OptionsMenuView : ModelsMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private SliderOptionMenuItem _masterVolume = null;
        [SerializeField] private DropdownOptionMenuItem _windowFrameType = null;
        [SerializeField] private DropdownOptionMenuItem _windowFrameColor = null;
        [SerializeField] private ModelsMenuViewItemButton _exitButton = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private static string[] windowFrameTypeNames;
        private static string[] windowFrameColorNames;

        private RogueOptions currentOptions;

        public void Initialize()
        {
            _exitButton.Initialize(this);
            _masterVolume.Initialize(x => currentOptions.SetMasterVolume(x));
            _windowFrameType.Initialize(x => currentOptions.SetWindowFrame(x, currentOptions.WindowFrameColor));
            _windowFrameColor.Initialize(x => currentOptions.SetWindowFrame(currentOptions.WindowFrameIndex, ColorPreset.GetColor(x)));
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models,
            IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(root, self, user, arg);

            if (windowFrameTypeNames == null)
            {
                windowFrameTypeNames = Enumerable.Range(0, WindowFrameList.Count).Select(i => WindowFrameList.GetName(i)).ToArray();
                windowFrameColorNames = Enumerable.Range(0, ColorPreset.Count).Select(i => ColorPreset.GetName(i)).ToArray();
            }

            var device = (StandardRogueDevice)RogueDevice.Primary;
            currentOptions = device.Options;
            _masterVolume.Open("マスター音量", currentOptions.MasterVolume);
            _windowFrameType.Open("ウィンドウタイプ", windowFrameTypeNames, currentOptions.WindowFrameIndex);
            _windowFrameColor.Open("ウィンドウカラー", windowFrameColorNames, ColorPreset.IndexOf(currentOptions.WindowFrameColor));

            MenuController.Show(_exitButton.CanvasGroup, false);
            MenuController.Show(_canvasGroup, true);
            ShowExitButton(ExitModelsMenuChoice.Instance);
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

        private void ShowExitButton(IModelsMenuChoice choice)
        {
            _exitButton.SetItem(ChoicesModelsMenuItemController.Instance, choice);
            MenuController.Show(_exitButton.CanvasGroup, true);
        }
    }
}
