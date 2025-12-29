using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <inheritdoc/>
    public class DropdownListMenuViewData<TItem, TMgr> : DropdownListMenuViewData<TItem, TMgr, IListuiArg>
        where TItem : class
        where TMgr : IListuiManager
    { }

    /// <summary>
    /// 項目のスクロールが必要なメニュー向け ViewData
    /// </summary>
    public class DropdownListMenuViewData<TItem, TMgr, TArg> : TreeViewData<TItem, TMgr, TArg>
        where TItem : class
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        public System.Func<TMgr, IListHandlerSubview> DropdownListSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.DropdownList;
        public System.Func<TMgr, IListHandlerSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler{TMgr, TArg}"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption{TMgr, TArg}"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy
        {
            get => dropdownListSubviewHandler.EnableSelectOptionProxy;
            set => dropdownListSubviewHandler.EnableSelectOptionProxy = value;
        }

        private object prevViewStateHolder;
        private ISubviewStateProvider dropdownListSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;

        private readonly TreeButtonViewItemHandler<TItem, TMgr, TArg> dropdownListSubviewHandler = new();
        private ListuiEventHandler<TMgr, TArg> onShow;
        private ListuiEventHandler onHide;

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
            dropdownListSubviewStateProvider?.Reset();
            captionBoxSubviewStateProvider?.Reset();
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
        {
            DropdownListSubviewSelector?.Invoke(manager)?.Show(
                List, dropdownListSubviewHandler, manager, arg, ref dropdownListSubviewStateProvider, onHide: onHide);

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    TitleSingle, ToStringViewItemHandler.Instance, manager, arg, ref captionBoxSubviewStateProvider);
            }

            // 上記の Show によって実行される onHide の後に onShow を呼び出す
            onShow?.Invoke(manager, arg);
        }

        public virtual void Hide(TMgr manager, bool back)
        {
            DropdownListSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
        }

        public class Builder :
            BaseListBuilder<DropdownListMenuViewData<TItem, TMgr, TArg>, Builder>,
            IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, Builder>,
            ITreeViewItemHandlerBuilder<TItem, TMgr, TArg, Builder>
        {
            public Builder(DropdownListMenuViewData<TItem, TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder OnShow(ListuiEventHandler<TMgr, TArg> handler)
            {
                AssertNotBuilt();

                Parent.onShow += handler;
                return this;
            }

            public Builder OnHide(ListuiEventHandler<TMgr, TArg> handler)
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

                if (Parent.dropdownListSubviewHandler.GetName != null) { Debug.LogWarning($"{nameof(NameFrom)} が多重購読されました。"); }

                Parent.dropdownListSubviewHandler.GetName += selector;
                return this;
            }

            public Builder StyleFrom(System.Func<TItem, TMgr, TArg, string> selector)
            {
                AssertNotBuilt();

                if (Parent.dropdownListSubviewHandler.GetStyle != null) { Debug.LogWarning($"{nameof(StyleFrom)} が多重購読されました。"); }

                Parent.dropdownListSubviewHandler.GetStyle += selector;
                return this;
            }

            public Builder OnClick(ClickItemHandler<TItem, TMgr, TArg> handler)
            {
                AssertNotBuilt();

                Parent.dropdownListSubviewHandler.Click += handler;
                return this;
            }

            public Builder ChildrenFrom(System.Func<TItem, TMgr, TArg, IReadOnlyList<TItem>> selector)
            {
                AssertNotBuilt();

                if (Parent.dropdownListSubviewHandler.GetStyle != null) { Debug.LogWarning($"{nameof(ChildrenFrom)} が多重購読されました。"); }

                Parent.dropdownListSubviewHandler.GetChildren += selector;
                return this;
            }

            protected override void Unload()
            {
                base.Unload();
                Parent.dropdownListSubviewHandler.GetName = null;
                Parent.dropdownListSubviewHandler.GetStyle = null;
                Parent.dropdownListSubviewHandler.Click = null;
                Parent.dropdownListSubviewHandler.GetChildren = null;
                Parent.onShow = null;
                Parent.onHide = null;
            }
        }
    }
}
