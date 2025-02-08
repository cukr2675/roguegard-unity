using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using TMPro;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/View Widgets/LMF Input Field View Widget")]
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldViewWidget : ViewWidget, ISelectHandler
    {
        private IWidgetOption widgetOption;
        private ElementsSubViewBase _parent;
        private TMP_InputField inputField;
        private bool queuedDeactivateInputField;

        public override string WidgetName => widgetOption.Name;
        protected override ElementsSubViewBase Parent => _parent;

        public delegate string HandleValueChanged<TMgr, TArg>(TMgr manager, TArg arg, string value);

        public override bool TryInstantiateWidget(
            object element, IElementHandler handler, ElementsSubViewBase elementsSubView, out ViewWidget viewWidget)
        {
            if (!(element is IWidgetOption widgetOption))
            {
                viewWidget = null;
                return false;
            }

            var inputFieldViewWidget = Instantiate(this);
            inputFieldViewWidget._parent = elementsSubView;
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
            GetElementName<TMgr, TArg> getValue, HandleValueChanged<TMgr, TArg> handleValueChanged,
            TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard, string name = null)
        {
            return new WidgetOption<TMgr, TArg>()
            {
                Name = name ?? EmitIdentity("InputFieldViewWidget"),
                ContentType = contentType,
                GetValue = getValue,
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
