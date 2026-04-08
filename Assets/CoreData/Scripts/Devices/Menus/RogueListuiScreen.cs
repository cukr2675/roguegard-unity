using Lysionium;

namespace Roguegard.Device
{
    public abstract class RogueListuiScreen : IListuiScreen<MMgrBase, MArg>
    {
        protected MArg Arg { get; private set; }
        protected event OpenScreenHandler OnOpenScreen;
        protected event CloseScreenViewHandler OnCloseScreenView;

        bool IListuiScreen.IsIncremental => OnCloseScreenView != null;

        public delegate void OpenScreenHandler(MMgr manager);
        public delegate void CloseScreenViewHandler(MMgr manager, bool back);

        /// <summary>
        /// <see cref="MMgrBase"/> の実装は <see cref="MMgr"/> を必ず継承することを想定するため、
        /// <see cref="MMgr"/> と合わせて安全ではないキャストを許容する
        /// </summary>
        void IListuiScreen<MMgrBase, MArg>.OpenScreen(MMgrBase manager, MArg arg)
        {
            if (OnOpenScreen == null) throw new System.InvalidOperationException($"{this}.{nameof(OnOpenScreen)} が設定されていません。");

            Arg = arg;
            OnOpenScreen((MMgr)manager);
        }

        /// <summary>
        /// <see cref="MMgrBase"/> の実装は <see cref="MMgr"/> を必ず継承することを想定するため、
        /// <see cref="MMgr"/> と合わせて安全ではないキャストを許容する
        /// </summary>
        void IListuiScreen<MMgrBase, MArg>.CloseScreenView(MMgrBase manager, bool back, MArg arg)
        {
            Arg = arg;
            if (OnCloseScreenView != null)
            {
                OnCloseScreenView((MMgr)manager, back);
            }
            else
            {
                manager.HideAll(back);
            }
        }
    }
}
