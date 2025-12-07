namespace Lysionium
{
    /// <summary>
    /// <see cref="IMenuScreen{TMgr, TArg}"/> の標準実装クラス
    /// </summary>
    public abstract class MenuScreen<TMgr> : IMenuScreen<TMgr, IListMenuArg>
        where TMgr : IListMenuManager
    {
        public virtual bool IsIncremental => false;

        public abstract void OpenScreen(TMgr manager, IListMenuArg arg);

        public virtual void CloseScreenView(TMgr manager, bool back) => manager.HideAll(back);
    }

    /// <summary>
    /// <see cref="IMenuScreen{TMgr, TArg}"/> の標準実装クラス
    /// </summary>
    public abstract class MenuScreen<TMgr, TArg> : IMenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public virtual bool IsIncremental => false;

        public abstract void OpenScreen(TMgr manager, TArg arg);

        public virtual void CloseScreenView(TMgr manager, bool back) => manager.HideAll(back);
    }
}
