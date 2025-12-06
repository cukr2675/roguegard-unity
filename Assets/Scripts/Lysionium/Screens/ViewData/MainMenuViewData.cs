using System.Collections.Generic;

namespace Lysionium
{
    /// <summary>
    /// 項目数が固定のメニュー向け ViewData
    /// </summary>
    public class MainMenuViewData<TMgr, TArg> : ListViewData<ISelectOption, TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public System.Func<TMgr, IListHandlerSubview> PrimaryCommandSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.PrimaryCommand;
        public System.Func<TMgr, IListHandlerSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<TMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

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
                List, SelectOptionViewItemHandler.Instance, manager, arg, ref primaryCommandSubviewStateProvider);

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    TitleSingle, ToStringViewItemHandler.Instance, manager, arg, ref captionBoxSubviewStateProvider);
            }

            BackAnchorSubviewSelector?.Invoke(manager)?.Show(
                BackAnchorList, SelectOptionViewItemHandler.Instance, manager, arg, ref backAnchorSubviewStateProvider);
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

            public Builder Option(ISelectOption option)
            {
                return Tail(option);
            }
        }
    }
}
