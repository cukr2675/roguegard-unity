using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// 項目数が可変のメニュー向け ViewTemplate
    /// </summary>
    public class CommandListViewTemplate<TElm, TMgr, TArg> : ListViewTemplate<TElm, TMgr, TArg>
        where TElm : class
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string SecodaryCommandSubviewName { get; set; } = StandardSubviewTable.SecondaryCommandName;
        public string CaptionBoxSubviewName { get; set; } = StandardSubviewTable.CaptionBoxName;
        public string BackAnchorSubviewName { get; set; } = null;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionHandler"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy
        {
            get => secodaryCommandSubviewHandler.EnableSelectOptionProxy;
            set => secodaryCommandSubviewHandler.EnableSelectOptionProxy = value;
        }

        private object prevViewStateHolder;
        private IElementsSubviewStateProvider secodaryCommandSubviewStateProvider;
        private IElementsSubviewStateProvider captionBoxSubviewStateProvider;
        private IElementsSubviewStateProvider backAnchorSubviewStateProvider;

        private readonly ButtonElementHandler<TElm, TMgr, TArg> secodaryCommandSubviewHandler = new();

        public Builder ShowTemplate(
            IReadOnlyList<TElm> list, TMgr manager, TArg arg, object viewStateHolder = null)
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
                    .Show(TitleSingle, ElementToStringHandler.Instance, manager, arg, ref captionBoxSubviewStateProvider);
            }

            if (BackAnchorSubviewName != null)
            {
                manager
                    .GetSubview(BackAnchorSubviewName)
                    .Show(BackAnchorList, SelectOptionHandler.Instance, manager, arg, ref backAnchorSubviewStateProvider);
            }
        }

        public void HideTemplate(TMgr manager, bool back)
        {
            manager.GetSubview(SecodaryCommandSubviewName).Hide(back);
            if (Title != null) { manager.GetSubview(CaptionBoxSubviewName).Hide(back); }
            if (BackAnchorSubviewName != null) { manager.GetSubview(BackAnchorSubviewName).Hide(back); }
        }

        public class Builder : BaseBuilder<Builder>, IButtonElementHandlerBuilder<TElm, TMgr, TArg, Builder>
        {
            private readonly CommandListViewTemplate<TElm, TMgr, TArg> parent;

            public Builder(CommandListViewTemplate<TElm, TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder NameFrom(GetElementName<TElm, TMgr, TArg> nameFrom)
            {
                AssertNotBuilt();

                if (parent.secodaryCommandSubviewHandler.GetName != null) { Debug.LogWarning($"{nameof(NameFrom)} が多重購読されました。"); }

                parent.secodaryCommandSubviewHandler.GetName += nameFrom;
                return this;
            }

            public Builder StyleFrom(GetElementStyle<TElm, TMgr, TArg> styleFrom)
            {
                AssertNotBuilt();

                if (parent.secodaryCommandSubviewHandler.GetStyle != null) { Debug.LogWarning($"{nameof(StyleFrom)} が多重購読されました。"); }

                parent.secodaryCommandSubviewHandler.GetStyle += styleFrom;
                return this;
            }

            public Builder OnClick(HandleClickElement<TElm, TMgr, TArg> onClick)
            {
                AssertNotBuilt();

                parent.secodaryCommandSubviewHandler.HandleClick += onClick;
                return this;
            }
        }
    }
}
