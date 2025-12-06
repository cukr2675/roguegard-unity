namespace Lysionium
{
    public interface IListMenuManager
    {
        ISelectOption BackOption { get; }

        ISelectOption ErrorOption { get; }

        event System.Action OnUnload;

        void HideAll(bool back);

        string Localize(string text);

        T Localize<T>(T obj);

        void PushMenuScreenFromExtension(object menuScreen, IListMenuArg arg);
    }
}
