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
        public System.Func<TMgr, IListHandlerSubview> WidgetsSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.Widgets;
        public System.Func<TMgr, IListHandlerSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<TMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.BackAnchor;
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
            WidgetsSubviewSelector?.Invoke(manager)?.Show(
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
            WidgetsSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder : BaseListBuilder<VariableWidgetsMenuViewData<TMgr, TArg>, Builder>
        {
            public Builder(VariableWidgetsMenuViewData<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder HeadStack(params object[] items)
            {
                return Head(StackWidgetOption.Create(items.Select(x => ("1*", x)).ToArray()));
            }

            public Builder TailStack(params object[] items)
            {
                return Tail(StackWidgetOption.Create(items.Select(x => ("1*", x)).ToArray()));
            }
        }
    }
}
