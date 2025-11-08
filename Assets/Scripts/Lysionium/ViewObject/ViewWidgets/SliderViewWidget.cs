using UnityEngine;
using UnityEngine.UI;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Widgets/LUI Input Field View Widget")]
    [RequireComponent(typeof(Slider))]
    public class SliderViewWidget : ViewWidget
    {
        private ISliderWidgetOption widgetOption;
        private SubviewBase _parent;
        private Slider slider;

        public override string WidgetName => widgetOption.Name;
        protected override SubviewBase Parent => _parent;


        public override bool TryInstantiateWidget(
            object item, IViewItemHandler handler, SubviewBase subview, out ViewWidget viewWidget)
        {
            if (item is not ISliderWidgetOption widgetOption)
            {
                viewWidget = null;
                return false;
            }

            var inputFieldViewWidget = Instantiate(this);
            inputFieldViewWidget._parent = subview;
            inputFieldViewWidget.widgetOption = widgetOption;
            inputFieldViewWidget.slider = inputFieldViewWidget.GetComponent<Slider>();
            inputFieldViewWidget.Initialize();
            viewWidget = inputFieldViewWidget;
            return true;
        }

        private void Initialize()
        {
            slider.minValue = widgetOption.MinValue;
            slider.maxValue = widgetOption.MaxValue;
            slider.SetValueWithoutNotify(widgetOption.GetValue(_parent.Manager, _parent.Arg));
            slider.onValueChanged.AddListener(value =>
            {
                slider.SetValueWithoutNotify(widgetOption.HandleValueChanged(_parent.Manager, _parent.Arg, value));
            });
        }
    }
}
