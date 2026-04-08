namespace Lysionium
{
    /// <inheritdoc/>
    public abstract class DelegateListuiScreen<TMgr> : DelegateListuiScreen<TMgr, IListuiArg>
        where TMgr : IListuiManager
    { }

    // プライマリコンストラクタ (C#12) が使えるようになったら廃止予定
    public abstract class DelegateListuiScreen<TMgr, TArg> : IListuiScreen<TMgr, TArg>
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        protected event OpenScreenHandler OnOpenScreen;
        protected event CloseScreenViewHandler OnCloseScreenView;

        bool IListuiScreen.IsIncremental => OnCloseScreenView != null;

        public delegate void OpenScreenHandler(TMgr manager, TArg arg);
        public delegate void CloseScreenViewHandler(TMgr manager, bool back);

        void IListuiScreen<TMgr, TArg>.OpenScreen(TMgr manager, TArg arg)
        {
            OnOpenScreen?.Invoke(manager, arg);
        }

        void IListuiScreen<TMgr, TArg>.CloseScreenView(TMgr manager, bool back)
        {
            if (OnCloseScreenView != null)
            {
                OnCloseScreenView(manager, back);
            }
            else
            {
                manager.HideAll(back);
            }
        }
    }

    public abstract class DelegateListuiScreen<TMgr, TArg, TCtx> : IListuiScreen<TMgr, TArg, TCtx>
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        protected event OpenScreenHandler OnOpenScreen;
        protected event CloseScreenViewHandler OnCloseScreenView;

        bool IListuiScreen.IsIncremental => OnCloseScreenView != null;

        public delegate void OpenScreenHandler(TMgr manager, TArg arg, TCtx context);
        public delegate void CloseScreenViewHandler(TMgr manager, bool back, TCtx context);

        void IListuiScreen<TMgr, TArg, TCtx>.OpenScreen(TMgr manager, TArg arg, TCtx context)
        {
            OnOpenScreen?.Invoke(manager, arg, context);
        }

        void IListuiScreen<TMgr, TArg, TCtx>.CloseScreenView(TMgr manager, bool back, TCtx context)
        {
            if (OnCloseScreenView != null)
            {
                OnCloseScreenView(manager, back, context);
            }
            else
            {
                manager.HideAll(back);
            }
        }
    }

#if UNITY_EDITOR
    // コラム: DelegateListuiScreen の没拡張案
    // ViewDataBuilder.Build() の戻り値を CloseScreenHandler にすることで CloseScreenView もオーバーライド不要にする
    // 処理フローが隠れるので可読性が低下する
    // ViewDataBuilder.Build() の戻り値は IDisposable にすべきではないか？
    internal abstract class DelegateListuiScreen2<TMgr, TArg> : IListuiScreen<TMgr, TArg>
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        protected event OpenScreenHandler OnOpenScreen;
        protected event OpenScreenOfIncrementalHandler OnOpenScreenOfIncremental;
        private event CloseScreenHandler OnCloseScreen;
        bool IListuiScreen.IsIncremental => OnCloseScreen != null;

        protected delegate void OpenScreenHandler(in TMgr manager, in TArg arg);
        protected delegate void CloseScreenHandler(TMgr manager, bool back);
        protected delegate CloseScreenHandler OpenScreenOfIncrementalHandler(in TMgr manager, in TArg arg);

        void IListuiScreen<TMgr, TArg>.OpenScreen(TMgr manager, TArg arg)
        {
            if (OnOpenScreen != null && OnOpenScreenOfIncremental != null) throw new System.InvalidOperationException();

            OnOpenScreen?.Invoke(manager, arg);
            OnCloseScreen += OnOpenScreenOfIncremental?.Invoke(manager, arg);
        }

        void IListuiScreen<TMgr, TArg>.CloseScreenView(TMgr manager, bool back)
        {
            OnCloseScreen?.Invoke(manager, back);
        }
    }
#endif
}
