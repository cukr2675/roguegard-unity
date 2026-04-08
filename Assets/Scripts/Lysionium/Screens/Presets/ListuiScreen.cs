namespace Lysionium
{
    /// <summary>
    /// <see cref="IListuiScreen{TMgr}"/> の標準実装クラス
    /// </summary>
    public abstract class ListuiScreen<TMgr> : IListuiScreen<TMgr>
        where TMgr : IListuiManager
    {
        public virtual bool IsIncremental => false;

        public abstract void OpenScreen(TMgr manager);

        public virtual void CloseScreenView(TMgr manager, bool back) => manager.HideAll(back);
    }
}
