using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// 項目のスクロールが必要なメニュー向け ViewData
    /// </summary>
    public class ScrollMenuViewData<TItem, TMgr> : ListViewData<TItem, TMgr>
        where TItem : class
        where TMgr : IListuiManager
    {
        public System.Func<TMgr, IListHandlerSubview> ScrollSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.Scroll;
        public System.Func<TMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<TMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.BackAnchor;
        public SelectOptionList<TMgr> BackAnchorList { get; set; } = new(_ => _.BackIfReflectable());

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler{TMgr}"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption{TMgr}"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy
        {
            get => scrollSubviewHandler.EnableSelectOptionProxy;
            set => scrollSubviewHandler.EnableSelectOptionProxy = value;
        }

        private object prevViewStateHolder;
        private ISubviewStateProvider scrollSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        private readonly ButtonViewItemHandler<TItem, TMgr> scrollSubviewHandler = new();

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
            scrollSubviewStateProvider?.Reset();
            captionBoxSubviewStateProvider?.Reset();
            backAnchorSubviewStateProvider?.Reset();
        }

        protected override void ShowSubviews(TMgr manager)
        {
            ScrollSubviewSelector?.Invoke(manager)?.Show(
                List, scrollSubviewHandler, manager, ref scrollSubviewStateProvider, onHide: OnHide);

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
            ScrollSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder : BaseListBuilder<ScrollMenuViewData<TItem, TMgr>, Builder>, IButtonViewItemHandlerBuilder<TItem, TMgr, Builder>
        {
            public Builder(ScrollMenuViewData<TItem, TMgr> parent, TMgr manager)
                : base(parent, manager)
            {
            }

            public Builder NameFrom(System.Func<TItem, TMgr, string> selector)
            {
                AssertNotBuilt();

                if (Parent.scrollSubviewHandler.GetName != null) { Debug.LogWarning($"{nameof(NameFrom)} が多重購読されました。"); }

                Parent.scrollSubviewHandler.GetName += selector;
                return this;
            }

            public Builder StyleFrom(System.Func<TItem, TMgr, string> selector)
            {
                AssertNotBuilt();

                if (Parent.scrollSubviewHandler.GetStyle != null) { Debug.LogWarning($"{nameof(StyleFrom)} が多重購読されました。"); }

                Parent.scrollSubviewHandler.GetStyle += selector;
                return this;
            }

            public Builder OnClick(ClickItemHandler<TItem, TMgr> handler)
            {
                AssertNotBuilt();

                Parent.scrollSubviewHandler.Click += handler;
                return this;
            }

            protected override void Unload()
            {
                base.Unload();
                Parent.scrollSubviewHandler.GetName = null;
                Parent.scrollSubviewHandler.GetStyle = null;
                Parent.scrollSubviewHandler.Click = null;
            }
        }
    }
}
