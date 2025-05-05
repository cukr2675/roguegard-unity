using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Widgets/Meta/LUI Content Size Meta Widget")]
    public class ContentSizeMetaWidget : ViewWidget
    {
        private ElementsSubviewBase _parent;
        protected override ElementsSubviewBase Parent => _parent;

        public override bool TryInstantiateWidget(
            object element, IElementHandler handler, ElementsSubviewBase elementsSubview, out ViewWidget viewWidget)
        {
            if (!(element is IWidgetOption widgetOption))
            {
                viewWidget = null;
                return false;
            }

            if (elementsSubview is WidgetsSubview widgetsSubview)
            {
                widgetsSubview.SetContentWidth(widgetOption.Width);
            }

            var metaWidget = Instantiate(this);
            metaWidget._parent = elementsSubview;
            viewWidget = metaWidget;
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
