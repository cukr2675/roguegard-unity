namespace Lysionium
{
    public static class SliderWidgetOption
    {
        public static ISliderWidgetOption Create<TMgr, TArg>(
            System.Func<TMgr, TArg, float> value, ISliderWidgetOption.SliderEventHandler<TMgr, TArg> onValueChanged,
            float minValue = 0f, float maxValue = 100f, string name = null)
        {
            return new WidgetOptionImplement<TMgr, TArg>()
            {
                Name = name ?? LuiUtility.EmitIdentity("SliderViewWidget"),
                MinValue = minValue,
                MaxValue = maxValue,
                GetValue = value,
                HandleValueChanged = onValueChanged
            };
        }

        private class WidgetOptionImplement<TMgr, TArg> : ISliderWidgetOption
        {
            public string Name { get; set; }
            public float MinValue { get; set; }
            public float MaxValue { get; set; }
            public System.Func<TMgr, TArg, float> GetValue { get; set; }
            public ISliderWidgetOption.SliderEventHandler<TMgr, TArg> HandleValueChanged { get; set; }

            float ISliderWidgetOption.GetValue(IListMenuManager manager, IListMenuArg arg)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return 0f;

                return GetValue(tMgr, tArg);
            }

            float ISliderWidgetOption.HandleValueChanged(IListMenuManager manager, IListMenuArg arg, float value)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return 0f;

                return HandleValueChanged(tMgr, tArg, value);
            }
        }
    }
}
