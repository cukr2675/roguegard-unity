using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public override CanvasGroup CanvasGroup => _canvasGroup;

        public override void OpenView<T>(
            IModelListPresenter presenter, Spanning<T> modelList, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(root, self, user, arg);
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
    }
}
