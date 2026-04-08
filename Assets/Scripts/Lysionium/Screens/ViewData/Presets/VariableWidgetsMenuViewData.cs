using System.Collections.Generic;
using System.Linq;

namespace Lysionium
{
    /// <summary>
    /// 項目数が可変のウィジェットメニュー向け ViewData
    /// </summary>
    public class VariableWidgetsMenuViewData<TMgr> : ListViewData<object, TMgr>
        where TMgr : IListuiManager
    {
        public System.Func<TMgr, IListHandlerSubview> WidgetsSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.Widgets;
        public System.Func<TMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<TMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.BackAnchor;
        public SelectOptionList<TMgr> BackAnchorList { get; set; } = new(_ => _.BackIfReflectable());

        private object prevViewStateHolder;
        private ISubviewStateProvider primaryCommandSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        public Builder Show(object[] widgetOptions, TMgr manager, object viewStateHolder = null)
        {
            SetOriginalList(widgetOptions, manager);
            return ShowCore(manager, viewStateHolder);
        }

        public Builder Show(IReadOnlyList<object> widgetOptions, TMgr manager, object viewStateHolder = null)
        {
            SetOriginalList(widgetOptions, manager);
            return ShowCore(manager, viewStateHolder);
        }

        public Builder Show(System.ReadOnlySpan<object> widgetOptions, TMgr manager, object viewStateHolder = null)
        {
            SetOriginalList(widgetOptions, manager);
            return ShowCore(manager, viewStateHolder);
        }

        private Builder ShowCore(TMgr manager, object viewStateHolder)
        {
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
            WidgetsSubviewSelector?.Invoke(manager)?.Show(
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
            WidgetsSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder : BaseListBuilder<VariableWidgetsMenuViewData<TMgr>, Builder>
        {
            public Builder(VariableWidgetsMenuViewData<TMgr> parent, TMgr manager)
                : base(parent, manager)
            {
            }

            public Builder HeadStack(params object[] items)
            {
                return Head.Append(StackWidgetOption.Create(items.Select(x => ("1*", x)).ToArray()));
            }

            public Builder TailStack(params object[] items)
            {
                return Tail.Append(StackWidgetOption.Create(items.Select(x => ("1*", x)).ToArray()));
            }
        }
    }

    //public static class WidgetListBuilderExtensions
    //{
    //    public static VariableWidgetsMenuViewData<TMgr>.Builder Stack<TMgr>(
    //        this VariableWidgetsMenuViewData<TMgr>.Builder.HeadBuilder builder, params object[] items)
    //        where TMgr : IListuiManager
    //    {
    //        return builder.Append(StackWidgetOption.Create(items.Select(x => ("1*", x)).ToArray()));
    //    }

    //    public static VariableWidgetsMenuViewData<TMgr>.Builder Stack<TMgr>(
    //        this VariableWidgetsMenuViewData<TMgr>.Builder.TailBuilder builder, params object[] items)
    //        where TMgr : IListuiManager
    //    {
    //        return builder.Append(StackWidgetOption.Create(items.Select(x => ("1*", x)).ToArray()));
    //    }
    //}
}
