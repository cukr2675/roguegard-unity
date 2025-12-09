namespace Lysionium
{
    public interface IListMenuManager
    {
        ISelectOption<IListMenuManager, IListMenuArg> ErrorOption { get; }

        event System.Action OnUnload;

        void HideAll(bool back);

        string Localize(string text);

        T Localize<T>(T obj);
    }
}
