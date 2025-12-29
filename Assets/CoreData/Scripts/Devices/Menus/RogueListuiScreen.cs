using Lysionium;

namespace Roguegard.Device
{
    public abstract class RogueListuiScreen : IListuiScreen<MMgrBase, MArg>
    {
        public virtual bool IsIncremental => false;

        /// <summary>
        /// <see cref="MMgrBase"/> の実装は <see cref="MMgr"/> を必ず継承することを想定するため、
        /// <see cref="MMgr"/> と合わせて安全ではないキャストを許容する
        /// </summary>
        void IListuiScreen<MMgrBase, MArg>.OpenScreen(MMgrBase manager, MArg arg) => OpenScreen((MMgr)manager, arg);
        public abstract void OpenScreen(MMgr manager, MArg arg);

        /// <summary>
        /// <see cref="MMgrBase"/> の実装は <see cref="MMgr"/> を必ず継承することを想定するため、
        /// <see cref="MMgr"/> と合わせて安全ではないキャストを許容する
        /// </summary>
        void IListuiScreen<MMgrBase, MArg>.CloseScreenView(MMgrBase manager, bool back) => CloseScreenView((MMgr)manager, back);
        public virtual void CloseScreenView(MMgr manager, bool back) => manager.HideAll(back);
    }
}
