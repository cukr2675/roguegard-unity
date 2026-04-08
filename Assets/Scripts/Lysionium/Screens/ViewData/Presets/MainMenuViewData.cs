namespace Lysionium
{
    /// <summary>
    /// 項目数が固定のメニュー向け ViewData
    /// </summary>
    public class MainMenuViewData<TMgr> : ListViewData<ISelectOption<TMgr>, TMgr>
        where TMgr : IListuiManager
    {
        public System.Func<TMgr, IListHandlerSubview> PrimaryCommandSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.PrimaryCommand;
        public System.Func<TMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<TMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
        public SelectOptionList<TMgr> BackAnchorList { get; set; } = new(_ => _.BackIfReflectable());

        private object prevViewStateHolder;
        private ISubviewStateProvider primaryCommandSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        public Builder Show(TMgr manager, object viewStateHolder = null)
        {
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder) { ResetSubviewStateProviders(); }
            prevViewStateHolder = viewStateHolder;

            if (TryShowSubviews(manager)) return null;
            else return new Builder(this, manager);
        }

        protected virtual void ResetSubviewStateProviders()
        {
            primaryCommandSubviewStateProvider?.Reset();
            captionBoxSubviewStateProvider?.Reset();
            backAnchorSubviewStateProvider?.Reset();
        }

        protected override void ShowSubviews(TMgr manager)
        {
            PrimaryCommandSubviewSelector?.Invoke(manager)?.Show(
                List, SelectOptionViewItemHandler<TMgr>.Instance, manager, ref primaryCommandSubviewStateProvider, onHide: OnHide);

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    Title, manager, ref captionBoxSubviewStateProvider);
            }

            BackAnchorSubviewSelector?.Invoke(manager)?.Show(
                BackAnchorList, manager, ref backAnchorSubviewStateProvider);
        }

        public virtual void Hide(TMgr manager, bool back)
        {
            PrimaryCommandSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder : BaseListBuilder<MainMenuViewData<TMgr>, Builder>, ISelectOptionsBuilder<TMgr, Builder>
        {
            public Builder(MainMenuViewData<TMgr> parent, TMgr manager)
                : base(parent, manager)
            {
            }

            public Builder Option(ISelectOption<TMgr> option)
            {
                return Tail.Option(option);
            }

            Builder ISelectOptionsBuilder<TMgr, Builder>.Option() => this;
        }
    }
}
