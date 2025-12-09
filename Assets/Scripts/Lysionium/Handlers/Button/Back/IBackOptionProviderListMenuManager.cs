namespace Lysionium
{
    public interface IBackOptionProviderListMenuManager<TMgr, TArg> : IListMenuManager
    {
        ISelectOption<TMgr, TArg> BackOption { get; }
    }
}
