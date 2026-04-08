namespace Lysionium
{
    /// <summary>
    /// <see cref="IListuiScreen{TMgr, TArg}"/> を表示可能な <see cref="IListuiManager"/>
    /// </summary>
    public interface IListuiScreenManager<TMgr, TArg> : IListuiManager
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        void PushScreen(IListuiScreen<TMgr, TArg> screen, TArg arg);

        void PushScreen<TCtx>(IListuiScreen<TMgr, TArg, TCtx> screen, TArg arg, TCtx context);
    }
}
