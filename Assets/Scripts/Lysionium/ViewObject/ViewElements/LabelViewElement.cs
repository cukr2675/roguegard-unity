using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Elements/LUI Label View Element")]
    [RequireComponent(typeof(TMP_Text))]
    public class LabelViewElement : ViewElement
    {
        private TMP_Text text;

        private void Awake()
        {
            TryGetComponent(out text);
        }

        protected override void InnerSetElement(object element, IElementHandler handler)
        {
            text.text = Manager.Localize(name);
        }
    }
}
