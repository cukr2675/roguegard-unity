namespace Lysionium
{
    public static class SliderWidgetOption
    {
        public static ISliderWidgetOption Create<TMgr>(
            System.Func<TMgr, float> value, ISliderWidgetOption.SliderEventHandler<TMgr> onValueChanged,
            float minValue = 0f, float maxValue = 100f, string name = null)
        {
            return new WidgetOptionImplement<TMgr>()
            {
                Name = name ?? LuiUtility.EmitIdentity("SliderViewWidget"),
                MinValue = minValue,
                MaxValue = maxValue,
                GetValue = value,
                HandleValueChanged = onValueChanged
            };
        }

        private class WidgetOptionImplement<TMgr> : ISliderWidgetOption
        {
            public string Name { get; set; }
            public float MinValue { get; set; }
            public float MaxValue { get; set; }
            public System.Func<TMgr, float> GetValue { get; set; }
            public ISliderWidgetOption.SliderEventHandler<TMgr> HandleValueChanged { get; set; }

            float ISliderWidgetOption.GetValue(IListuiManager manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return 0f;

                return GetValue(tMgr);
            }

            float ISliderWidgetOption.HandleValueChanged(IListuiManager manager, float value)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return 0f;

                return HandleValueChanged(tMgr, value);
            }
        }
    }
}
