using TMPro;

namespace Lysionium
{
    public interface IInputFieldWidgetOption
    {
        string Name { get; }

        TMP_InputField.ContentType ContentType { get; }

        public delegate string InputFieldEventHandler<TMgr, TArg>(TMgr manager, TArg arg, string value);

        string GetValue(IListMenuManager manager, IListMenuArg arg);

        string HandleValueChanged(IListMenuManager manager, IListMenuArg arg, string value);
    }
}
