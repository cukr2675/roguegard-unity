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
        public string PrimaryCommandSubviewName { get; set; } = StandardSubviewTable.PrimaryCommandName;
        public string CaptionBoxSubviewName { get; set; } = StandardSubviewTable.CaptionBoxName;
        public string BackAnchorSubviewName { get; set; } = null;
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
            manager
                .GetSubview(PrimaryCommandSubviewName)
                .Show(List, SelectOptionViewItemHandler.Instance, manager, arg, ref primaryCommandSubviewStateProvider);

            if (Title != null)
            {
                manager
                    .GetSubview(CaptionBoxSubviewName)
                    .Show(TitleSingle, ToStringViewItemHandler.Instance, manager, arg, ref captionBoxSubviewStateProvider);
            }

            if (BackAnchorSubviewName != null)
            {
                manager
                    .GetSubview(BackAnchorSubviewName)
                    .Show(BackAnchorList, SelectOptionViewItemHandler.Instance, manager, arg, ref backAnchorSubviewStateProvider);
            }
        }

        public virtual void Hide(TMgr manager, bool back)
        {
            manager.GetSubview(PrimaryCommandSubviewName).Hide(back);
            if (Title != null) { manager.GetSubview(CaptionBoxSubviewName).Hide(back); }
            if (BackAnchorSubviewName != null) { manager.GetSubview(BackAnchorSubviewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<MainMenuViewData<TMgr, TArg>, Builder>
        {
            public Builder(MainMenuViewData<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder Option(string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
            {
                return Tail(SelectOption.Create(name, onClick, style));
            }

            public Builder Back()
            {
                return Tail(BackSelectOption.Instance);
            }
        }
    }
}
