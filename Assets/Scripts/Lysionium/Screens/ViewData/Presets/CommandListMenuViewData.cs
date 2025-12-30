using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <inheritdoc/>
    public class CommandListMenuViewData<TItem, TMgr> : CommandListMenuViewData<TItem, TMgr, IListuiArg>
        where TItem : class
        where TMgr : IListuiManager
    { }

    /// <summary>
    /// 項目数が可変のメニュー向け ViewData
    /// </summary>
    public class CommandListMenuViewData<TItem, TMgr, TArg> : ListViewData<TItem, TMgr, TArg>
        where TItem : class
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        public System.Func<TMgr, IListHandlerSubview> SecondaryCommandSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.SecondaryCommand;
        public System.Func<TMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<TMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
        public SelectOptionList<TMgr, TArg> BackAnchorList { get; set; } = new(_ => _.BackIfReflectable());

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler{TMgr, TArg}"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption{TMgr, TArg}"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy
        {
            get => secondaryCommandSubviewHandler.EnableSelectOptionProxy;
            set => secondaryCommandSubviewHandler.EnableSelectOptionProxy = value;
        }

        private object prevViewStateHolder;
        private ISubviewStateProvider secondaryCommandSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        private readonly ButtonViewItemHandler<TItem, TMgr, TArg> secondaryCommandSubviewHandler = new();

        public Builder Show(TItem[] list, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            SetOriginalList(list, manager, arg);
            return ShowCore(manager, arg, viewStateHolder);
        }

        public Builder Show(IReadOnlyList<TItem> list, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            SetOriginalList(list, manager, arg);
            return ShowCore(manager, arg, viewStateHolder);
        }

        public Builder Show(System.ReadOnlySpan<TItem> list, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            SetOriginalList(list, manager, arg);
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
            secondaryCommandSubviewStateProvider?.Reset();
            captionBoxSubviewStateProvider?.Reset();
            backAnchorSubviewStateProvider?.Reset();
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
        {
            SecondaryCommandSubviewSelector?.Invoke(manager)?.Show(
                List, secondaryCommandSubviewHandler, manager, arg, ref secondaryCommandSubviewStateProvider);

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
            SecondaryCommandSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder
            : BaseListBuilder<CommandListMenuViewData<TItem, TMgr, TArg>, Builder>, IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, Builder>
        {
            public Builder(CommandListMenuViewData<TItem, TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder NameFrom(System.Func<TItem, TMgr, TArg, string> selector)
            {
                AssertNotBuilt();

                if (Parent.secondaryCommandSubviewHandler.GetName != null) { Debug.LogWarning($"{nameof(NameFrom)} が多重購読されました。"); }

                Parent.secondaryCommandSubviewHandler.GetName += selector;
                return this;
            }

            public Builder StyleFrom(System.Func<TItem, TMgr, TArg, string> selector)
            {
                AssertNotBuilt();

                if (Parent.secondaryCommandSubviewHandler.GetStyle != null) { Debug.LogWarning($"{nameof(StyleFrom)} が多重購読されました。"); }

                Parent.secondaryCommandSubviewHandler.GetStyle += selector;
                return this;
            }

            public Builder OnClick(ClickItemHandler<TItem, TMgr, TArg> handler)
            {
                AssertNotBuilt();

                Parent.secondaryCommandSubviewHandler.Click += handler;
                return this;
            }

            protected override void Unload()
            {
                base.Unload();
                Parent.secondaryCommandSubviewHandler.GetName = null;
                Parent.secondaryCommandSubviewHandler.GetStyle = null;
                Parent.secondaryCommandSubviewHandler.Click = null;
            }
        }
    }
}
