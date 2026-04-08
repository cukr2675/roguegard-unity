using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// 項目のスクロールが必要なメニュー向け ViewData
    /// </summary>
    public class DropdownListMenuViewData<TItem, TMgr> : TreeViewData<TItem, TMgr>
        where TItem : class
        where TMgr : IListuiManager
    {
        public System.Func<TMgr, IListHandlerSubview> DropdownListSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.DropdownList;
        public System.Func<TMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler{TMgr}"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption{TMgr}"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy
        {
            get => dropdownListSubviewHandler.EnableSelectOptionProxy;
            set => dropdownListSubviewHandler.EnableSelectOptionProxy = value;
        }

        private object prevViewStateHolder;
        private ISubviewStateProvider dropdownListSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;

        private readonly TreeButtonViewItemHandler<TItem, TMgr> dropdownListSubviewHandler = new();

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
            dropdownListSubviewStateProvider?.Reset();
            captionBoxSubviewStateProvider?.Reset();
        }

        protected override void ShowSubviews(TMgr manager)
        {
            DropdownListSubviewSelector?.Invoke(manager)?.Show(
                List, dropdownListSubviewHandler, manager, ref dropdownListSubviewStateProvider, onHide: OnHide);

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    Title, manager, ref captionBoxSubviewStateProvider);
            }
        }

        public virtual void Hide(TMgr manager, bool back)
        {
            DropdownListSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
        }

        public class Builder :
            BaseListBuilder<DropdownListMenuViewData<TItem, TMgr>, Builder>,
            IButtonViewItemHandlerBuilder<TItem, TMgr, Builder>,
            ITreeViewItemHandlerBuilder<TItem, TMgr, Builder>
        {
            public Builder(DropdownListMenuViewData<TItem, TMgr> parent, TMgr manager)
                : base(parent, manager)
            {
            }

            public Builder NameFrom(System.Func<TItem, TMgr, string> selector)
            {
                AssertNotBuilt();

                if (Parent.dropdownListSubviewHandler.GetName != null) { Debug.LogWarning($"{nameof(NameFrom)} が多重購読されました。"); }

                Parent.dropdownListSubviewHandler.GetName += selector;
                return this;
            }

            public Builder StyleFrom(System.Func<TItem, TMgr, string> selector)
            {
                AssertNotBuilt();

                if (Parent.dropdownListSubviewHandler.GetStyle != null) { Debug.LogWarning($"{nameof(StyleFrom)} が多重購読されました。"); }

                Parent.dropdownListSubviewHandler.GetStyle += selector;
                return this;
            }

            public Builder OnClick(ClickItemHandler<TItem, TMgr> handler)
            {
                AssertNotBuilt();

                Parent.dropdownListSubviewHandler.Click += handler;
                return this;
            }

            public Builder ChildrenFrom(System.Func<TItem, TMgr, IReadOnlyList<TItem>> selector)
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
            }
        }
    }
}
