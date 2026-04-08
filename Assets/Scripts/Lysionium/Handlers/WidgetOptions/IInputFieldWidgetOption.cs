using TMPro;

namespace Lysionium
{
    public interface IInputFieldWidgetOption
    {
        string Name { get; }

        TMP_InputField.ContentType ContentType { get; }

        delegate string InputFieldEventHandler(string value);
        delegate string InputFieldEventHandler<in TMgr>(string value, TMgr manager);

        string GetValue(IListuiManager manager);

        string HandleValueChanged(string value, IListuiManager manager);
    }
}
