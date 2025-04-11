using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/View Widgets/Headers/LMF Style Header View Widget")]
    public class StyleSizeHeaderViewWidget : ViewWidget
    {
        private ElementsSubViewBase _parent;
        protected override ElementsSubViewBase Parent => _parent;

        public override bool TryInstantiateWidget(
            object element, IElementHandler handler, ElementsSubViewBase elementsSubView, out ViewWidget viewWidget)
        {
            if (!(element is IWidgetOption widgetOption))
            {
                viewWidget = null;
                return false;
            }

            if (elementsSubView is WidgetsSubView widgetsSubView)
            {
                widgetsSubView.SetStyle(widgetOption.Style);
            }

            var headerViewWidget = Instantiate(this);
            headerViewWidget._parent = elementsSubView;
            viewWidget = headerViewWidget;
            return true;
        }

        public static IWidgetOption CreateOption(string style)
        {
            return new WidgetOption()
            {
                Style = style,
            };
        }

        public interface IWidgetOption
        {
            string Style { get; }
        }

        private class WidgetOption : IWidgetOption
        {
            public string Style { get; set; }
        }
    }
}
