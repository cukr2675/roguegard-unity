namespace Lysionium
{
    public abstract class DelegateListuiScreen<TMgr> : IListuiScreen<TMgr>
        where TMgr : IListuiManager
    {
        protected event OpenScreenHandler OnOpenScreen;
        protected event CloseScreenViewHandler OnCloseScreenView;

        bool IListuiScreen.IsIncremental => OnCloseScreenView != null;

        public delegate void OpenScreenHandler(TMgr manager);
        public delegate void CloseScreenViewHandler(TMgr manager, bool back);

        void IListuiScreen<TMgr>.OpenScreen(TMgr manager)
        {
            if (OnOpenScreen == null) throw new System.InvalidOperationException($"{this}.{nameof(OnOpenScreen)} が設定されていません。");

            OnOpenScreen(manager);
        }

        void IListuiScreen<TMgr>.CloseScreenView(TMgr manager, bool back)
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

    public abstract class DelegateListuiScreen<TMgr, TArg> : IListuiScreen<TMgr, TArg>
        where TMgr : IListuiManager
    {
        protected TArg Arg { get; private set; }
        protected event OpenScreenHandler OnOpenScreen;
        protected event CloseScreenViewHandler OnCloseScreenView;

        bool IListuiScreen.IsIncremental => OnCloseScreenView != null;

        public delegate void OpenScreenHandler(TMgr manager);
        public delegate void CloseScreenViewHandler(TMgr manager, bool back);

        void IListuiScreen<TMgr, TArg>.OpenScreen(TMgr manager, TArg arg)
        {
            if (OnOpenScreen == null) throw new System.InvalidOperationException($"{this}.{nameof(OnOpenScreen)} が設定されていません。");

            Arg = arg;
            OnOpenScreen(manager);
        }

        void IListuiScreen<TMgr, TArg>.CloseScreenView(TMgr manager, bool back, TArg arg)
        {
            Arg = arg;
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

#if UNITY_EDITOR
    // コラム: DelegateListuiScreen の没拡張案
    // ViewDataBuilder.Build() の戻り値を CloseScreenHandler にすることで CloseScreenView もオーバーライド不要にする
    // 処理フローが隠れるので可読性が低下する
    // ViewDataBuilder.Build() の戻り値は IDisposable にすべきではないか？
    internal abstract class DelegateListuiScreen2<TMgr> : IListuiScreen<TMgr>
        where TMgr : IListuiManager
    {
        protected event OpenScreenHandler OnOpenScreen;
        protected event OpenScreenOfIncrementalHandler OnOpenScreenOfIncremental;
        private event CloseScreenHandler OnCloseScreen;
        bool IListuiScreen.IsIncremental => OnCloseScreen != null;

        protected delegate void OpenScreenHandler(in TMgr manager);
        protected delegate void CloseScreenHandler(TMgr manager, bool back);
        protected delegate CloseScreenHandler OpenScreenOfIncrementalHandler(in TMgr manager);

        void IListuiScreen<TMgr>.OpenScreen(TMgr manager)
        {
            if (OnOpenScreen != null && OnOpenScreenOfIncremental != null) throw new System.InvalidOperationException();

            OnOpenScreen?.Invoke(manager);
            OnCloseScreen += OnOpenScreenOfIncremental?.Invoke(manager);
        }

        void IListuiScreen<TMgr>.CloseScreenView(TMgr manager, bool back)
        {
            OnCloseScreen?.Invoke(manager, back);
        }
    }
#endif
}
