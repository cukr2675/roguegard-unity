namespace Lysionium
{
    /// <summary>
    /// <see cref="IListuiScreen{TMgr, TArg}"/> の標準実装クラス
    /// </summary>
    public abstract class ListuiScreen<TMgr> : IListuiScreen<TMgr, IListuiArg>
        where TMgr : IListuiManager
    {
        public virtual bool IsIncremental => false;

        public abstract void OpenScreen(TMgr manager, IListuiArg arg);

        public virtual void CloseScreenView(TMgr manager, bool back) => manager.HideAll(back);
    }

    /// <summary>
    /// <see cref="IListuiScreen{TMgr, TArg}"/> の標準実装クラス
    /// </summary>
    public abstract class ListuiScreen<TMgr, TArg> : IListuiScreen<TMgr, TArg>
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        public virtual bool IsIncremental => false;

        public abstract void OpenScreen(TMgr manager, TArg arg);

        public virtual void CloseScreenView(TMgr manager, bool back) => manager.HideAll(back);
    }
}
