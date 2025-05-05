using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// 項目のスクロールが必要なメニュー向け ViewTemplate
    /// </summary>
    public class ScrollViewTemplate<TElm, TMgr, TArg> : ListViewTemplate<TElm, TMgr, TArg>
        where TElm : class
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string ScrollSubviewName { get; set; } = StandardSubviewTable.ScrollName;
        public string CaptionBoxSubviewName { get; set; } = StandardSubviewTable.CaptionBoxName;
        public string BackAnchorSubviewName { get; set; } = StandardSubviewTable.BackAnchorName;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionHandler"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy
        {
            get => scrollSubviewHandler.EnableSelectOptionProxy;
            set => scrollSubviewHandler.EnableSelectOptionProxy = value;
        }

        private object prevViewStateHolder;
        private IElementsSubviewStateProvider scrollSubviewStateProvider;
        private IElementsSubviewStateProvider captionBoxSubviewStateProvider;
        private IElementsSubviewStateProvider backAnchorSubviewStateProvider;

        private readonly ButtonElementHandler<TElm, TMgr, TArg> scrollSubviewHandler = new();

        public Builder ShowTemplate(
            IReadOnlyList<TElm> list, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder) { ResetSubviewStateProviders(); }
            prevViewStateHolder = viewStateHolder;

            // スクロールのビューを表示
            OriginalList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                OriginalList.Add(list[i]);
            }

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
                .Show(List, scrollSubviewHandler, manager, arg, ref scrollSubviewStateProvider);

            if (Title != null)
            {
                manager
                    .GetSubview(CaptionBoxSubviewName)
                    .Show(TitleSingle, ElementToStringHandler.Instance, manager, arg, ref captionBoxSubviewStateProvider);
            }

            if (BackAnchorSubviewName != null)
            {
                manager
                    .GetSubview(BackAnchorSubviewName)
                    .Show(BackAnchorList, SelectOptionHandler.Instance, manager, arg, ref backAnchorSubviewStateProvider);
            }
        }

        public virtual void HideTemplate(TMgr manager, bool back)
        {
            manager.GetSubview(ScrollSubviewName).Hide(back);
            if (Title != null) { manager.GetSubview(CaptionBoxSubviewName).Hide(back); }
            if (BackAnchorSubviewName != null) { manager.GetSubview(BackAnchorSubviewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<Builder>, IButtonElementHandlerBuilder<TElm, TMgr, TArg, Builder>
        {
            private readonly ScrollViewTemplate<TElm, TMgr, TArg> parent;

            public Builder(ScrollViewTemplate<TElm, TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder NameFrom(GetElementName<TElm, TMgr, TArg> nameFrom)
            {
                AssertNotBuilt();

                if (parent.scrollSubviewHandler.GetName != null) { Debug.LogWarning($"{nameof(NameFrom)} が多重購読されました。"); }

                parent.scrollSubviewHandler.GetName += nameFrom;
                return this;
            }

            public Builder StyleFrom(GetElementStyle<TElm, TMgr, TArg> styleFrom)
            {
                AssertNotBuilt();

                if (parent.scrollSubviewHandler.GetStyle != null) { Debug.LogWarning($"{nameof(StyleFrom)} が多重購読されました。"); }

                parent.scrollSubviewHandler.GetStyle += styleFrom;
                return this;
            }

            public Builder OnClick(HandleClickElement<TElm, TMgr, TArg> onClick)
            {
                AssertNotBuilt();

                parent.scrollSubviewHandler.HandleClick += onClick;
                return this;
            }
        }
    }
}
