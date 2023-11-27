using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using UnityEngine.UI;
using Roguegard;
using Roguegard.Device;
using TMPro;

namespace RoguegardUnity
{
    public class DetailsMenuView : ModelsMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private ModelsMenuViewItemButton _exitButton = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        public void Initialize()
        {
            _exitButton.Initialize(this);
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models,
            IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(root, self, user, arg);
            MenuController.Show(_exitButton.CanvasGroup, false);
            MenuController.Show(_canvasGroup, true);
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

        public void SetObj(RogueObj obj)
        {
            var details = obj.Main.InfoSet.Details;
            if (details != null)
            {
                _text.text = details.ToString();
                return;
            }

            var name = obj.Main.InfoSet.Name;
            if (name.StartsWith(':'))
            {
                _text.text = StandardRogueDeviceUtility.Localize($"{name}::d");
                return;
            }
        }

        public void ShowExitButton(IModelsMenuChoice choice)
        {
            _exitButton.SetItem(ChoicesModelsMenuItemController.Instance, choice);
            MenuController.Show(_exitButton.CanvasGroup, true);
        }
    }
}
