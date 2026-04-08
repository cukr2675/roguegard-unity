namespace Lysionium
{
    /// <summary>
    /// <see cref="IListuiScreen{TMgr}"/> を表示可能な <see cref="IListuiManager"/>
    /// </summary>
    public interface IListuiScreenManager<TMgr> : IListuiManager
        where TMgr : IListuiManager
    {
        void PushScreen(IListuiScreen<TMgr> screen);

        void PushScreen<TArg>(IListuiScreen<TMgr, TArg> screen, TArg arg);
    }
}
