using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// 項目数が可変のメニュー向け ViewData
    /// </summary>
    public class CommandListViewData<TItem, TMgr, TArg> : ListViewData<TItem, TMgr, TArg>
        where TItem : class
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string SecodaryCommandSubviewName { get; set; } = StandardSubviewTable.SecondaryCommandName;
        public string CaptionBoxSubviewName { get; set; } = StandardSubviewTable.CaptionBoxName;
        public string BackAnchorSubviewName { get; set; } = null;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy
        {
            get => secodaryCommandSubviewHandler.EnableSelectOptionProxy;
            set => secodaryCommandSubviewHandler.EnableSelectOptionProxy = value;
        }

        private object prevViewStateHolder;
        private ISubviewStateProvider secodaryCommandSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        private readonly ButtonViewItemHandler<TItem, TMgr, TArg> secodaryCommandSubviewHandler = new();

        public Builder Show(IReadOnlyList<TItem> list, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                secodaryCommandSubviewStateProvider?.Reset();
                captionBoxSubviewStateProvider?.Reset();
                backAnchorSubviewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            OriginalList.Clear();
            OriginalList.AddRange(list);

            if (TryShowSubviews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
        {
            manager
                .GetSubview(SecodaryCommandSubviewName)
                .Show(List, secodaryCommandSubviewHandler, manager, arg, ref secodaryCommandSubviewStateProvider);

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
            manager.GetSubview(SecodaryCommandSubviewName).Hide(back);
            if (Title != null) { manager.GetSubview(CaptionBoxSubviewName).Hide(back); }
            if (BackAnchorSubviewName != null) { manager.GetSubview(BackAnchorSubviewName).Hide(back); }
        }

        public class Builder : BaseBuilder<Builder>, IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, Builder>
        {
            private readonly CommandListViewData<TItem, TMgr, TArg> parent;

            public Builder(CommandListViewData<TItem, TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder NameFrom(ItemNameSelector<TItem, TMgr, TArg> selector)
            {
                AssertNotBuilt();

                if (parent.secodaryCommandSubviewHandler.GetName != null) { Debug.LogWarning($"{nameof(NameFrom)} が多重購読されました。"); }

                parent.secodaryCommandSubviewHandler.GetName += selector;
                return this;
            }

            public Builder StyleFrom(ItemStyleSelector<TItem, TMgr, TArg> selector)
            {
                AssertNotBuilt();

                if (parent.secodaryCommandSubviewHandler.GetStyle != null) { Debug.LogWarning($"{nameof(StyleFrom)} が多重購読されました。"); }

                parent.secodaryCommandSubviewHandler.GetStyle += selector;
                return this;
            }

            public Builder OnClick(ClickItemHandler<TItem, TMgr, TArg> handler)
            {
                AssertNotBuilt();

                parent.secodaryCommandSubviewHandler.Click += handler;
                return this;
            }
        }
    }
}
