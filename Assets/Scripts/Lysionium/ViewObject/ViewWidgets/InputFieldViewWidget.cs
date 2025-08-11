using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/View Widgets/LUI Input Field View Widget")]
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldViewWidget : ViewWidget, ISelectHandler
    {
        private IWidgetOption widgetOption;
        private SubviewBase _parent;
        private TMP_InputField inputField;
        private bool queuedDeactivateInputField;

        public override string WidgetName => widgetOption.Name;
        protected override SubviewBase Parent => _parent;

        public delegate string InputFieldEventHandler<TMgr, TArg>(TMgr manager, TArg arg, string value);

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
            inputFieldViewWidget.inputField = inputFieldViewWidget.GetComponent<TMP_InputField>();
            inputFieldViewWidget.Initialize();
            viewWidget = inputFieldViewWidget;
            return true;
        }

        private void Initialize()
        {
            inputField.contentType = widgetOption.ContentType;
            inputField.SetTextWithoutNotify(widgetOption.GetValue(_parent.Manager, _parent.Arg));
            inputField.onValueChanged.AddListener(value =>
            {
                inputField.SetTextWithoutNotify(widgetOption.HandleValueChanged(_parent.Manager, _parent.Arg, value));
            });
        }

        public static IWidgetOption CreateOption<TMgr, TArg>(
            System.Func<TMgr, TArg, string> value, InputFieldEventHandler<TMgr, TArg> handleValueChanged,
            TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard, string name = null)
        {
            return new WidgetOption<TMgr, TArg>()
            {
                Name = name ?? EmitIdentity("InputFieldViewWidget"),
                ContentType = contentType,
                GetValue = value,
                HandleValueChanged = handleValueChanged
            };
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            queuedDeactivateInputField = true;
        }

        private void LateUpdate()
        {
            if (queuedDeactivateInputField)
            {
                inputField.DeactivateInputField();
                queuedDeactivateInputField = false;
            }
        }

        public interface IWidgetOption
        {
            string Name { get; }

            TMP_InputField.ContentType ContentType { get; }

            string GetValue(IListMenuManager manager, IListMenuArg arg);

            string HandleValueChanged(IListMenuManager manager, IListMenuArg arg, string value);
        }

        private class WidgetOption<TMgr, TArg> : IWidgetOption
        {
            public string Name { get; set; }
            public TMP_InputField.ContentType ContentType { get; set; }
            public System.Func<TMgr, TArg, string> GetValue { get; set; }
            public InputFieldEventHandler<TMgr, TArg> HandleValueChanged { get; set; }

            string IWidgetOption.GetValue(IListMenuManager manager, IListMenuArg arg)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return null;

                return GetValue(tMgr, tArg);
            }

            string IWidgetOption.HandleValueChanged(IListMenuManager manager, IListMenuArg arg, string value)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return null;

                return HandleValueChanged(tMgr, tArg, value);
            }
        }
    }
}
