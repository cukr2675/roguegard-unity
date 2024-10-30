using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/View Widgets/LMF Input Field View Widget")]
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldViewWidget : ViewWidget
    {
        private ElementsSubView parent;
        private IWidgetOption widgetOption;
        private TMP_InputField inputField;

        public delegate string HandleValueChanged<TMgr, TArg>(TMgr manager, TArg arg, string value);

        public override bool TryInstantiateWidget(
            ElementsSubView elementsSubView, IElementHandler handler, object element, out ViewWidget viewWidget)
        {
            if (!(element is IWidgetOption widgetOption))
            {
                viewWidget = null;
                return false;
            }

            var inputFieldViewWidget = Instantiate(this);
            inputFieldViewWidget.parent = elementsSubView;
            inputFieldViewWidget.widgetOption = widgetOption;
            inputFieldViewWidget.inputField = inputFieldViewWidget.GetComponent<TMP_InputField>();
            inputFieldViewWidget.Initialize();
            viewWidget = inputFieldViewWidget;
            return true;
        }

        private void Initialize()
        {
            inputField.contentType = widgetOption.ContentType;
            inputField.SetTextWithoutNotify(widgetOption.GetValue(parent.Manager, parent.Arg));
            inputField.onValueChanged.AddListener(value =>
            {
                inputField.SetTextWithoutNotify(widgetOption.HandleValueChanged(parent.Manager, parent.Arg, value));
            });
        }

        public static IWidgetOption CreateOption<TMgr, TArg>(
            GetElementName<TMgr, TArg> getValue, HandleValueChanged<TMgr, TArg> handleValueChanged,
            TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard)
        {
            return new WidgetOption<TMgr, TArg>()
            {
                ContentType = contentType,
                GetValue = getValue,
                HandleValueChanged = handleValueChanged
            };
        }

        public interface IWidgetOption
        {
            TMP_InputField.ContentType ContentType { get; }

            string GetValue(IListMenuManager manager, IListMenuArg arg);

            string HandleValueChanged(IListMenuManager manager, IListMenuArg arg, string value);
        }

        private class WidgetOption<TMgr, TArg> : IWidgetOption
        {
            public TMP_InputField.ContentType ContentType { get; set; }
            public GetElementName<TMgr, TArg> GetValue { get; set; }
            public HandleValueChanged<TMgr, TArg> HandleValueChanged { get; set; }

            string IWidgetOption.GetValue(IListMenuManager manager, IListMenuArg arg)
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                    LMFAssert.Type<TArg>(arg, out var tArg)) return null;

                return GetValue(tMgr, tArg);
            }

            string IWidgetOption.HandleValueChanged(IListMenuManager manager, IListMenuArg arg, string value)
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                    LMFAssert.Type<TArg>(arg, out var tArg)) return null;

                return HandleValueChanged(tMgr, tArg, value);
            }
        }
    }
}
