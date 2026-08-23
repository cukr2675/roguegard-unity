using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// 項目数が可変のメニュー向け ViewData
    /// </summary>
    public class CommandListMenuViewData<TItem, TMgr> : ListViewData<TItem, TMgr>
        where TItem : class
        where TMgr : IListuiManager
    {
        public System.Func<TMgr, IListHandlerSubview> SecondaryCommandSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.SecondaryCommand;
        public System.Func<TMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<TMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
        public SelectOptionList<TMgr> BackAnchorList { get; set; } = new(_ => _.BackIfReflectable());

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler{TMgr}"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption{TMgr}"/> を入れる場合を想定)
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

        private readonly EventGestureViewItemHandler<TItem, TMgr> secondaryCommandSubviewHandler = new();

        public Builder Show(TItem[] list, TMgr manager, object viewStateHolder = null)
        {
            SetOriginalList(list, manager);
            return ShowCore(manager, viewStateHolder);
        }

        public Builder Show(IReadOnlyList<TItem> list, TMgr manager, object viewStateHolder = null)
        {
            SetOriginalList(list, manager);
            return ShowCore(manager, viewStateHolder);
        }

        public Builder Show(System.ReadOnlySpan<TItem> list, TMgr manager, object viewStateHolder = null)
        {
            SetOriginalList(list, manager);
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
            secondaryCommandSubviewStateProvider?.Reset();
            captionBoxSubviewStateProvider?.Reset();
            backAnchorSubviewStateProvider?.Reset();
        }

        protected override void ShowSubviews(TMgr manager)
        {
            SecondaryCommandSubviewSelector?.Invoke(manager)?.Show(
                List, secondaryCommandSubviewHandler, manager, ref secondaryCommandSubviewStateProvider, onHide: OnHide);

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
            SecondaryCommandSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder :
            BaseListBuilder<CommandListMenuViewData<TItem, TMgr>, Builder>,
            IEventGestureViewItemHandlerBuilder<TItem, TMgr, Builder>
        {
            public Builder(CommandListMenuViewData<TItem, TMgr> parent, TMgr manager)
                : base(parent, manager)
            {
            }

            public Builder NameFrom(System.Func<TItem, TMgr, string> selector)
            {
                AssertNotBuilt();

                if (Parent.secondaryCommandSubviewHandler.GetName != null) { Debug.LogWarning($"{nameof(NameFrom)} が多重購読されました。"); }

                Parent.secondaryCommandSubviewHandler.GetName += selector;
                return this;
            }

            public Builder StyleFrom(System.Func<TItem, TMgr, string> selector)
            {
                AssertNotBuilt();

                if (Parent.secondaryCommandSubviewHandler.GetStyle != null) { Debug.LogWarning($"{nameof(StyleFrom)} が多重購読されました。"); }

                Parent.secondaryCommandSubviewHandler.GetStyle += selector;
                return this;
            }

            public Builder OnEventGestureConfirmed(string eventGestureName, SubmitItemHandler<TItem, TMgr> handler)
            {
                AssertNotBuilt();

                Parent.secondaryCommandSubviewHandler.SubscribeEventGestureConfirmed(eventGestureName, handler);
                return this;
            }

            protected override void Unload()
            {
                base.Unload();
                Parent.secondaryCommandSubviewHandler.GetName = null;
                Parent.secondaryCommandSubviewHandler.GetStyle = null;
                Parent.secondaryCommandSubviewHandler.ClearEventGestureConfirmed();
            }
        }
    }
}
