using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class RogueButtonViewElement : ViewElement
    {
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private TMP_Text _stackText = null;
        [SerializeField] private TMP_Text _info1Text = null;
        [SerializeField] private TMP_Text _info2Text = null;

        private IButtonElementHandler handler;
        private object element;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if (IsBlocked) return;

                handler.HandleClick(element, Manager, Arg);
            });
        }

        public override void SetElement(IElementHandler handler, object element)
        {
            this.handler = handler as IButtonElementHandler;
            this.element = element;

            var color = RoguegardSettings.White;
            Sprite icon = null;
            var iconColor = RoguegardSettings.White;
            int? stack = null;
            float? stars = null;
            string infoText1 = null;
            string infoText2 = null;
            if (handler is IRogueElementHandler rogueElementHandler &&
                Manager is RogueMenuManager manager &&
                Arg is ReadOnlyMenuArg arg)
            {
                rogueElementHandler.GetRogueInfo(
                    element, manager, arg, out var name, ref color, ref icon, ref iconColor, ref stack, ref stars, ref infoText1, ref infoText2);
                _nameText.text = Manager.Localize(name);
            }
            else
            {
                var baseText = handler.GetName(element, Manager, Arg);
                _nameText.text = Manager.Localize(baseText);
            }
            _nameText.color = color;

            if (icon != null)
            {
                _icon.sprite = icon;
                _icon.color = iconColor;
                _icon.enabled = true;
            }
            else
            {
                _icon.enabled = false;
            }

            _stackText.text = stack.ToString();
            _info1Text.text = infoText1;
            _info2Text.text = infoText2;
        }
    }
}
