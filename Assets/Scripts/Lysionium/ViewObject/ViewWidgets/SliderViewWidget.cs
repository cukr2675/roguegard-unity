using UnityEngine;
using UnityEngine.UI;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Widgets/LUI Input Field View Widget")]
    [RequireComponent(typeof(Slider))]
    public class SliderViewWidget : ViewWidget
    {
        private IWidgetOption widgetOption;
        private SubviewBase _parent;
        private Slider slider;

        public override string WidgetName => widgetOption.Name;
        protected override SubviewBase Parent => _parent;

        public delegate float SliderEventHandler<TMgr, TArg>(TMgr manager, TArg arg, float value);

        public override bool TryInstantiateWidget(
            object item, IViewItemHandler handler, SubviewBase subview, out ViewWidget viewWidget)
        {
            if (item is not IWidgetOption widgetOption)
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

        public static IWidgetOption CreateOption<TMgr, TArg>(
            System.Func<TMgr, TArg, float> value, SliderEventHandler<TMgr, TArg> onValueChanged,
            float minValue = 0f, float maxValue = 100f, string name = null)
        {
            return new WidgetOption<TMgr, TArg>()
            {
                Name = name ?? EmitIdentity("SliderViewWidget"),
                MinValue = minValue,
                MaxValue = maxValue,
                GetValue = value,
                HandleValueChanged = onValueChanged
            };
        }

        public interface IWidgetOption
        {
            string Name { get; }

            float MinValue { get; }

            float MaxValue { get; }

            float GetValue(IListMenuManager manager, IListMenuArg arg);

            float HandleValueChanged(IListMenuManager manager, IListMenuArg arg, float value);
        }

        private class WidgetOption<TMgr, TArg> : IWidgetOption
        {
            public string Name { get; set; }
            public float MinValue { get; set; }
            public float MaxValue { get; set; }
            public System.Func<TMgr, TArg, float> GetValue { get; set; }
            public SliderEventHandler<TMgr, TArg> HandleValueChanged { get; set; }

            float IWidgetOption.GetValue(IListMenuManager manager, IListMenuArg arg)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return 0f;

                return GetValue(tMgr, tArg);
            }

            float IWidgetOption.HandleValueChanged(IListMenuManager manager, IListMenuArg arg, float value)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return 0f;

                return HandleValueChanged(tMgr, tArg, value);
            }
        }
    }
}
