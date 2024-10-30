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

        private void Awake()
        {
            TryGetComponent(out text);
        }

        public override void SetElement(IElementHandler handler, object element)
        {
            var baseText = handler.GetName(element, Manager, Arg);
            text.text = Manager.Localize(baseText);
        }
    }
}
