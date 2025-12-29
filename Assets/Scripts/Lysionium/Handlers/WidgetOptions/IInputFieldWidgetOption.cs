using TMPro;

namespace Lysionium
{
    public interface IInputFieldWidgetOption
    {
        string Name { get; }

        TMP_InputField.ContentType ContentType { get; }

        delegate string InputFieldEventHandler<in TMgr, in TArg>(TMgr manager, TArg arg, string value);

        string GetValue(IListuiManager manager, IListuiArg arg);

        string HandleValueChanged(IListuiManager manager, IListuiArg arg, string value);
    }
}
