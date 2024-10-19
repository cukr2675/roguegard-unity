using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace ListingMF
{
    /// <summary>
    /// 複数のウィジェットを横並びに表示する <see cref="ViewWidget"/>
    /// </summary>
    [AddComponentMenu("UI/Listing Menu Foundation/View Widgets/LMF Label View Widget")]
    [RequireComponent(typeof(TMP_Text))]
    public class LabelViewWidget : ViewWidget
    {
        private TMP_Text text;

        public override bool TryInstantiateWidget(
            ElementsSubView elementsSubView, IElementHandler handler, object element, out ViewWidget viewWidget)
        {
            if (!(element is string name))
            {
                viewWidget = null;
                return false;
            }

            var labelViewWidget = Instantiate(this);
            labelViewWidget.text = labelViewWidget.GetComponent<TMP_Text>();
            labelViewWidget.text.text = name;
            viewWidget = labelViewWidget;
            return true;
        }
    }
}
