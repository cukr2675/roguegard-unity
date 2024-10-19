using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/View Elements/LMF Label View Element")]
    [RequireComponent(typeof(TMP_Text))]
    public class LabelViewElement : ViewElement
    {
        private TMP_Text text;

        private IButtonElementHandler handler;
        private object element;

        private void Awake()
        {
            TryGetComponent(out text);
        }

        public override void SetElement(IElementHandler handler, object element)
        {
            this.handler = handler as IButtonElementHandler;
            this.element = element;

            text.text = handler.GetName(element, Manager, Arg);
            //_text.color = new Color32(240, 240, 240, 255);
            //if (element is IListMenuIcon icon)
            //{
            //    icon.GetIcon(view.Root, view.Self, view.User, view.Arg, out var sprite, out var color);
            //    _icon.sprite = sprite;
            //    _icon.color = color;
            //    _icon.SetNativeSize();
            //    _icon.enabled = true;
            //}
        }
    }
}
