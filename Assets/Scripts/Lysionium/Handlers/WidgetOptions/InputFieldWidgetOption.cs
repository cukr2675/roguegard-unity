using TMPro;

namespace Lysionium
{
    public static class InputFieldWidgetOption
    {
        public static IInputFieldWidgetOption Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> value, IInputFieldWidgetOption.InputFieldEventHandler<TMgr, TArg> handleValueChanged,
            TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard, string name = null)
        {
            return new WidgetOptionImplement<TMgr, TArg>()
            {
                Name = name ?? LuiUtility.EmitIdentity("InputFieldViewWidget"),
                ContentType = contentType,
                GetValue = value,
                HandleValueChanged = handleValueChanged
            };
        }

        private class WidgetOptionImplement<TMgr, TArg> : IInputFieldWidgetOption
        {
            public string Name { get; set; }
            public TMP_InputField.ContentType ContentType { get; set; }
            public System.Func<TMgr, TArg, string> GetValue { get; set; }
            public IInputFieldWidgetOption.InputFieldEventHandler<TMgr, TArg> HandleValueChanged { get; set; }

            string IInputFieldWidgetOption.GetValue(IListuiManager manager, IListuiArg arg)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return null;

                return GetValue(tMgr, tArg);
            }

            string IInputFieldWidgetOption.HandleValueChanged(IListuiManager manager, IListuiArg arg, string value)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return null;

                return HandleValueChanged(tMgr, tArg, value);
            }
        }
    }
}
