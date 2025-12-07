namespace Lysionium
{
    /// <summary>
    /// <see cref="IMenuScreen{TMgr, TArg}"/> を表示可能な <see cref="IListMenuManager"/>
    /// </summary>
    public interface IMenuScreenListMenuManager<TMgr, TArg> : IListMenuManager
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        void PushMenuScreen(IMenuScreen<TMgr, TArg> menuScreen, TArg arg);
    }
}
