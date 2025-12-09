using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.Samples
{
    public class BindableScrollMenuViewData<TItem, TMgr, TArg> : ListViewData<TItem, TMgr, TArg>
        where TItem : class
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public System.Func<TMgr, IListHandlerSubview> ScrollSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.Scroll;
        public System.Func<TMgr, IListHandlerSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<TMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.BackAnchor;
        public SelectOptionList<TMgr, TArg> BackAnchorList { get; set; } = new(_ => _.BackIfReflectable());

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

        private readonly BindableButtonViewItemHandler<TItem, TMgr, TArg> scrollSubviewHandler = new();
        private ListMenuEventHandler<TMgr, TArg> onShow;
        private ListMenuEventHandler onHide;

        private TMgr manager;
        private TArg arg;

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

            this.manager = manager;
            this.arg = arg;

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
            ScrollSubviewSelector?.Invoke(manager)?.Show(
                List, scrollSubviewHandler, manager, arg, ref scrollSubviewStateProvider, onHide: onHide);

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    TitleSingle, ToStringViewItemHandler.Instance, manager, arg, ref captionBoxSubviewStateProvider);
            }

            BackAnchorSubviewSelector?.Invoke(manager)?.Show(
                BackAnchorList, manager, arg, ref backAnchorSubviewStateProvider);

            // 上記の Show によって実行される onHide の後に onShow を呼び出す
            onShow?.Invoke(manager, arg);
        }

        public virtual void Hide(TMgr manager, bool back)
        {
            ScrollSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder : BaseListBuilder<BindableScrollMenuViewData<TItem, TMgr, TArg>, Builder>, IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, Builder>
        {
            public Builder(BindableScrollMenuViewData<TItem, TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder VarOnceRebindList<T>(out T rebindList, System.Func<System.Action<IReadOnlyList<TItem>>, T> func)
            {
                AssertNotBuilt();

                var parent = Parent;
                rebindList = func(list =>
                {
                    // 再バインド用にリストを更新
                    parent.SetOriginalList(list, parent.manager, parent.arg);

                    // 再バインド対象の Subview のみ更新する（ShowSubviews を呼び出すとダイアログなどで上書きされた他の Subview も更新してしまう）
                    parent.ScrollSubviewSelector?.Invoke(Manager)?.SetParameters(
                        parent.List, parent.scrollSubviewHandler, parent.manager, parent.arg, ref parent.scrollSubviewStateProvider);
                });
                return this;
            }

            public Builder BinderFrom<TBinder>(System.Func<NotifyItemHandler, TBinder> selector)
                where TBinder : IDataBinder
            {
                AssertNotBuilt();

                if (Parent.scrollSubviewHandler.GetBinder != null) { Debug.LogWarning($"{nameof(BinderFrom)} が多重購読されました。"); }

                Parent.scrollSubviewHandler.GetBinder += (notify, cache) =>
                {
                    if (!cache.TryGet<TBinder>(out var binder))
                    {
                        binder = selector(notify);
                        cache.Add(binder);
                    }
                    return binder;
                };
                return this;
            }

            public Builder OnShow(ListMenuEventHandler<TMgr, TArg> handler)
            {
                AssertNotBuilt();

                Parent.onShow += handler;
                return this;
            }

            public Builder OnHide(ListMenuEventHandler<TMgr, TArg> handler)
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
