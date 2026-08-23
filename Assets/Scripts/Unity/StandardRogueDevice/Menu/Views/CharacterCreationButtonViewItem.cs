using Lysionium;
using Lysionium.Views;
using Roguegard;
using Roguegard.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoguegardUnity
{
    public class CharacterCreationButtonViewItem : ViewItem, IPointerClickHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private Image _background = null;
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private CharacterCreationStarsItem _stars = null;
        [SerializeField] private TMP_Text _captionText = null;
        [Space]
        [SerializeField] private Color _colorMinusStars = Color.black;
        [SerializeField] private Color _color0Stars = Color.gray;
        [SerializeField] private Color _color1Star = new Color(.8f, .5f, .3f) * lightRatio; // Bronze
        [SerializeField] private Color _color2Stars = new Color(.8f, .8f, .9f) * lightRatio; // Silver
        [SerializeField] private Color _color3Stars = new Color(.9f, .8f, .3f) * lightRatio; // Gold
        [SerializeField] private Color _colorManyStars = new Color(.7f, 1f, .8f) * lightRatio;

        private const float lightRatio = 248f / 255f;

        private IEventGestureViewItemHandler presenter;

        private object source;

        public CanvasGroup CanvasGroup => _canvasGroup;

        public void SetItem(
            IEventGestureViewItemHandler presenter, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData)
        {
            this.presenter = presenter;
            source = intrinsic;
            _icon.enabled = false;

            var text = presenter.GetName(intrinsic, Manager);
            text = StandardRogueDeviceUtility.Localize(text);
            var lv = intrinsic.Option.GetLv(intrinsic, characterCreationData);
            _text.text = $"Lv{lv} {text}";

            var cost = intrinsic.Option.GetCost(intrinsic, characterCreationData, out var costIsUnknown);
            var stars = RoguegardCharacterCreationSettings.GetItemStars(cost);
            _stars.SetStars(stars, costIsUnknown);
            _background.color = GetColor(stars);

            ShowCaption(intrinsic.Option);
        }

        public void SetItem(IEventGestureViewItemHandler presenter, IReadOnlyStartingItem startingItem)
        {
            this.presenter = presenter;
            source = startingItem;
            _icon.enabled = false;

            var text = presenter.GetName(startingItem, Manager);
            text = StandardRogueDeviceUtility.Localize(text);
            _text.text = text;

            var cost = startingItem.Option.GetCost(startingItem, out var costIsUnknown);
            var stars = RoguegardCharacterCreationSettings.GetItemStars(cost);
            _stars.SetStars(stars, costIsUnknown);
            _background.color = GetColor(stars);

            ShowCaption(startingItem.Option);
        }

        public void SetItem(IEventGestureViewItemHandler presenter, string text, string captionText)
        {
            this.presenter = presenter;
            source = null;
            _icon.enabled = false;
            _stars.SetInvisible();

            _text.text = text;
            _captionText.text = captionText;
        }

        private Color GetColor(int stars)
        {
            if (stars <= -1) return _colorMinusStars;
            if (stars == 0) return _color0Stars;
            if (stars == 1) return _color1Star;
            if (stars == 2) return _color2Stars;
            if (stars == 3) return _color3Stars;
            return _colorManyStars;
        }

        private void ShowCaption(IRogueDescribable describable)
        {
            var text = describable.Caption;
            if (string.IsNullOrWhiteSpace(text))
            {
                var name = describable.Name;
                if (name.StartsWith(':'))
                {
                    if (!StandardRogueDeviceUtility.TryLocalize($"{name}::c", out text) &&
                        string.IsNullOrWhiteSpace(text) &&
                        !StandardRogueDeviceUtility.TryLocalize($"{name}::d", out text))
                    {
                        text = "";
                    }
                }
            }

            _captionText.SetText(text);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            presenter.EventGestureConfirmed(source, Manager, "Click");
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            throw new System.NotImplementedException();
        }

        protected override void UnbindCore(object item, IViewItemHandler handler)
        {
            throw new System.NotImplementedException();
        }
    }
}
