using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Widgets/Meta/LUI Style Meta Widget")]
    public class StyleMetaWidget : ViewWidget
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
                widgetsSubview.SetStyle(widgetOption.Style);
            }

            var metaWidget = Instantiate(this);
            metaWidget._parent = elementsSubview;
            viewWidget = metaWidget;
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
