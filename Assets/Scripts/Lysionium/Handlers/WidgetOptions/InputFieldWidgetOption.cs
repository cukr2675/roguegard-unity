using TMPro;

namespace Lysionium
{
    public static class InputFieldWidgetOption
    {
        public static IInputFieldWidgetOption Create<TMgr>(
            System.Func<TMgr, string> value, IInputFieldWidgetOption.InputFieldEventHandler<TMgr> handleValueChanged,
            TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard, string name = null)
        {
            return new WidgetOptionImplement<TMgr>()
            {
                Name = name ?? LuiUtility.EmitIdentity("InputFieldViewWidget"),
                ContentType = contentType,
                GetValue = value,
                HandleValueChanged = handleValueChanged
            };
        }

        public static IInputFieldWidgetOption Create<TMgr>(
            System.Func<TMgr, string> value, IInputFieldWidgetOption.InputFieldEventHandler handleValueChanged,
            TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard, string name = null)
        {
            return new WidgetOptionImplement<TMgr>()
            {
                Name = name ?? LuiUtility.EmitIdentity("InputFieldViewWidget"),
                ContentType = contentType,
                GetValue = value,
                HandleValueChanged = (value, _) => handleValueChanged(value)
            };
        }

        private class WidgetOptionImplement<TMgr> : IInputFieldWidgetOption
        {
            public string Name { get; set; }
            public TMP_InputField.ContentType ContentType { get; set; }
            public System.Func<TMgr, string> GetValue { get; set; }
            public IInputFieldWidgetOption.InputFieldEventHandler<TMgr> HandleValueChanged { get; set; }

            string IInputFieldWidgetOption.GetValue(IListuiManager manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return null;

                return GetValue(tMgr);
            }

            string IInputFieldWidgetOption.HandleValueChanged(string value, IListuiManager manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return null;

                return HandleValueChanged(value, tMgr);
            }
        }
    }
}
