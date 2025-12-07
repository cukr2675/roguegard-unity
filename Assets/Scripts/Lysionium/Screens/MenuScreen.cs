namespace Lysionium
{
    /// <summary>
    /// <see cref="IMenuScreen{TMgr, TArg}"/> の標準実装クラス
    /// </summary>
    public abstract class MenuScreen<TMgr, TArg> : IMenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public virtual bool IsIncremental => false;

        public abstract void OpenScreen(in TMgr manager, in TArg arg);
        void IMenuScreen<TMgr, TArg>.OpenScreen(TMgr manager, TArg arg) => OpenScreen(manager, arg);

        public virtual void CloseScreenView(TMgr manager, bool back)
        {
            manager.HideAll(back);
        }
    }
}
