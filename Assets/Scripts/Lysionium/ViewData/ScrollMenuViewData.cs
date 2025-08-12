using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// 項目のスクロールが必要なメニュー向け ViewData
    /// </summary>
    public class ScrollMenuViewData<TItem, TMgr, TArg> : ListViewData<TItem, TMgr, TArg>
        where TItem : class
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string ScrollSubviewName { get; set; } = StandardSubviewTable.ScrollName;
        public string CaptionBoxSubviewName { get; set; } = StandardSubviewTable.CaptionBoxName;
        public string BackAnchorSubviewName { get; set; } = StandardSubviewTable.BackAnchorName;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption"/> を入れる場合を想定)
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

        private readonly ButtonViewItemHandler<TItem, TMgr, TArg> scrollSubviewHandler = new();
        private LuiEventHandler onShow;
        private LuiEventHandler onHide;

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
            scrollSubviewStateProvider?.Reset();
            captionBoxSubviewStateProvider?.Reset();
            backAnchorSubviewStateProvider?.Reset();
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
        {
            manager
                .GetSubview(ScrollSubviewName)
                .Show(List, scrollSubviewHandler, manager, arg, ref scrollSubviewStateProvider, onHide: onHide);

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

            // 上記の Show によって実行される onHide の後に onShow を呼び出す
            onShow?.Invoke(manager, arg);
        }

        public virtual void Hide(TMgr manager, bool back)
        {
            manager.GetSubview(ScrollSubviewName).Hide(back);
            if (Title != null) { manager.GetSubview(CaptionBoxSubviewName).Hide(back); }
            if (BackAnchorSubviewName != null) { manager.GetSubview(BackAnchorSubviewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<ScrollMenuViewData<TItem, TMgr, TArg>, Builder>, IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, Builder>
        {
            public Builder(ScrollMenuViewData<TItem, TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder OnShow(LuiEventHandler<TMgr, TArg> handler)
            {
                AssertNotBuilt();

                Parent.onShow += (manager, arg) =>
                {
                    if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                        LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

                    handler(tMgr, tArg);
                };
                return this;
            }

            public Builder OnHide(LuiEventHandler<TMgr, TArg> handler)
            {
                AssertNotBuilt();

                Parent.onHide += (manager, arg) =>
                {
                    if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                        LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

                    handler(tMgr, tArg);
                };
                return this;
            }

            public Builder NameFrom(System.Func<TItem, TMgr, TArg, string> selector)
            {
                AssertNotBuilt();

                if (Parent.scrollSubviewHandler.GetName != null) { Debug.LogWarning($"{nameof(NameFrom)} が多重購読されました。"); }

                Parent.scrollSubviewHandler.GetName += selector;
                return this;
            }

            public Builder StyleFrom(System.Func<TItem, TMgr, TArg, string> selector)
            {
                AssertNotBuilt();

                if (Parent.scrollSubviewHandler.GetStyle != null) { Debug.LogWarning($"{nameof(StyleFrom)} が多重購読されました。"); }

                Parent.scrollSubviewHandler.GetStyle += selector;
                return this;
            }

            public Builder OnClick(ClickItemHandler<TItem, TMgr, TArg> handler)
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
                Parent.onShow = null;
                Parent.onHide = null;
            }
        }
    }
}
