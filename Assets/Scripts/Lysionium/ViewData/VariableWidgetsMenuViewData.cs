using System.Collections.Generic;
using System.Linq;

namespace Lysionium
{
    /// <summary>
    /// 項目数が可変のウィジェットメニュー向け ViewData
    /// </summary>
    public class VariableWidgetsMenuViewData<TMgr, TArg> : ListViewData<object, TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string WidgetsSubviewName { get; set; } = StandardSubviewTable.WidgetsName;
        public string CaptionBoxSubviewName { get; set; } = StandardSubviewTable.CaptionBoxName;
        public string BackAnchorSubviewName { get; set; } = StandardSubviewTable.BackAnchorName;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        private object prevViewStateHolder;
        private ISubviewStateProvider primaryCommandSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        public Builder Show(object[] widgetOptions, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            SetOriginalList(widgetOptions, manager, arg);
            return ShowCore(manager, arg, viewStateHolder);
        }

        public Builder Show(IReadOnlyList<object> widgetOptions, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            SetOriginalList(widgetOptions, manager, arg);
            return ShowCore(manager, arg, viewStateHolder);
        }

        public Builder Show(System.ReadOnlySpan<object> widgetOptions, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            SetOriginalList(widgetOptions, manager, arg);
            return ShowCore(manager, arg, viewStateHolder);
        }

        private Builder ShowCore(TMgr manager, TArg arg, object viewStateHolder)
        {
            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                primaryCommandSubviewStateProvider?.Reset();
                captionBoxSubviewStateProvider?.Reset();
                backAnchorSubviewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            if (TryShowSubviews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
        {
            manager
                .GetSubview(WidgetsSubviewName)
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

        public void Hide(TMgr manager, bool back)
        {
            manager.GetSubview(WidgetsSubviewName).Hide(back);
            if (Title != null) { manager.GetSubview(CaptionBoxSubviewName).Hide(back); }
            if (BackAnchorSubviewName != null) { manager.GetSubview(BackAnchorSubviewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<VariableWidgetsMenuViewData<TMgr, TArg>, Builder>
        {
            public Builder(VariableWidgetsMenuViewData<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder HeadStack(params object[] items)
            {
                return Head(StackViewWidget.CreateOption(items.Select(x => ("1*", x)).ToArray()));
            }

            public Builder TailStack(params object[] items)
            {
                return Tail(StackViewWidget.CreateOption(items.Select(x => ("1*", x)).ToArray()));
            }
        }
    }
}
