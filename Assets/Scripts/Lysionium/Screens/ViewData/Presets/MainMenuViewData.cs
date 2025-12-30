namespace Lysionium
{
    /// <inheritdoc/>
    public class MainMenuViewData<TMgr> : MainMenuViewData<TMgr, IListuiArg>
        where TMgr : IListuiManager
    { }

    /// <summary>
    /// 項目数が固定のメニュー向け ViewData
    /// </summary>
    public class MainMenuViewData<TMgr, TArg> : ListViewData<ISelectOption<TMgr, TArg>, TMgr, TArg>
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        public System.Func<TMgr, IListHandlerSubview> PrimaryCommandSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.PrimaryCommand;
        public System.Func<TMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<TMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
        public SelectOptionList<TMgr, TArg> BackAnchorList { get; set; } = new(_ => _.BackIfReflectable());

        private object prevViewStateHolder;
        private ISubviewStateProvider primaryCommandSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        public Builder Show(TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder) { ResetSubviewStateProviders(); }
            prevViewStateHolder = viewStateHolder;

            if (TryShowSubviews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected virtual void ResetSubviewStateProviders()
        {
            primaryCommandSubviewStateProvider?.Reset();
            captionBoxSubviewStateProvider?.Reset();
            backAnchorSubviewStateProvider?.Reset();
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
        {
            PrimaryCommandSubviewSelector?.Invoke(manager)?.Show(
                List, SelectOptionViewItemHandler<TMgr, TArg>.Instance, manager, arg, ref primaryCommandSubviewStateProvider);

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    Title, manager, arg, ref captionBoxSubviewStateProvider);
            }

            BackAnchorSubviewSelector?.Invoke(manager)?.Show(
                BackAnchorList, manager, arg, ref backAnchorSubviewStateProvider);
        }

        public virtual void Hide(TMgr manager, bool back)
        {
            PrimaryCommandSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder : BaseListBuilder<MainMenuViewData<TMgr, TArg>, Builder>, ISelectOptionListBuilder<TMgr, TArg, Builder>
        {
            public Builder(MainMenuViewData<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder Option(ISelectOption<TMgr, TArg> option)
            {
                return Tail.Option(option);
            }

            Builder ISelectOptionListBuilder<TMgr, TArg, Builder>.Option() => this;
        }
    }
}
