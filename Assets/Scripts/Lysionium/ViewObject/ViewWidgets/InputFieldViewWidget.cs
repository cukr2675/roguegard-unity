using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/View Widgets/LUI Input Field View Widget")]
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldViewWidget : ViewWidget, ISelectHandler
    {
        private IInputFieldWidgetOption widgetOption;
        private SubviewBase _parent;
        private TMP_InputField inputField;
        private bool queuedDeactivateInputField;

        public override string WidgetName => widgetOption.Name;
        protected override SubviewBase Parent => _parent;

        public override bool TryInstantiateWidget(
            object item, IViewItemHandler handler, SubviewBase subview, out ViewWidget viewWidget)
        {
            if (item is not IInputFieldWidgetOption widgetOption)
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
            inputField.SetTextWithoutNotify(widgetOption.GetValue(_parent.Manager));
            inputField.onValueChanged.AddListener(value =>
            {
                inputField.SetTextWithoutNotify(widgetOption.HandleValueChanged(value, _parent.Manager));
            });
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            queuedDeactivateInputField = true;
        }

        protected virtual void LateUpdate()
        {
            if (queuedDeactivateInputField)
            {
                inputField.DeactivateInputField();
                queuedDeactivateInputField = false;
            }
        }
    }
}
