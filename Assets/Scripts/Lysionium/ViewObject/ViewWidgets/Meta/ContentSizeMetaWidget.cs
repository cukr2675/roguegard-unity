using UnityEngine;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Widgets/Meta/LUI Content Size Meta Widget")]
    public class ContentSizeMetaWidget : ViewWidget
    {
        private SubviewBase _parent;
        protected override SubviewBase Parent => _parent;

        public override bool TryInstantiateWidget(
            object item, IViewItemHandler handler, SubviewBase subview, out ViewWidget viewWidget)
        {
            if (item is not IWidgetOption widgetOption)
            {
                viewWidget = null;
                return false;
            }

            if (subview is WidgetsSubview widgetsSubview)
            {
                widgetsSubview.SetContentWidth(widgetOption.Width);
            }

            var metaWidget = Instantiate(this);
            metaWidget._parent = subview;
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
