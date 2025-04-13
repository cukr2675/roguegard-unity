using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Widgets/Headers/LUI Content Size Header View Widget")]
    public class ContentSizeHeaderViewWidget : ViewWidget
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
                widgetsSubView.SetContentWidth(widgetOption.Width);
            }

            var headerViewWidget = Instantiate(this);
            headerViewWidget._parent = elementsSubView;
            viewWidget = headerViewWidget;
            return true;
        }

        public static IWidgetOption CreateOption(float width)
        {
            return new WidgetOption()
            {
                Width = width,
            };
        }

        public interface IWidgetOption
        {
            float Width { get; }
        }

        private class WidgetOption : IWidgetOption
        {
            public float Width { get; set; }
        }
    }
}
