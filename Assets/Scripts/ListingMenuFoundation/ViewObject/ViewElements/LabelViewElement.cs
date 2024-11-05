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

        protected override void InnerSetElement(IElementHandler handler, object element)
        {
            text.text = Manager.Localize(name);
        }
    }
}
