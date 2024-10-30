using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/View Elements/LMF Button View Element")]
    [RequireComponent(typeof(Button))]
    public class ButtonViewElement : ViewElement
    {
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _text = null;

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

            var baseText = handler.GetName(element, Manager, Arg);
            _text.text = Manager.Localize(baseText);

            //if (icon != null)
            //{
            //    _icon.sprite = icon;
            //    _icon.SetNativeSize();
            //    _icon.enabled = true;
            //}
            //else
            //{
            //    _icon.sprite = null;
            //    _icon.enabled = false;
            //}
        }
    }
}
