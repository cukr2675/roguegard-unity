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
        public string SecodaryCommandSubViewName { get; set; } = StandardSubViewTable.SecondaryCommandName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;
        public string BackAnchorSubViewName { get; set; } = null;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionHandler"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy
        {
            get => secodaryCommandSubViewHandler.EnableSelectOptionProxy;
            set => secodaryCommandSubViewHandler.EnableSelectOptionProxy = value;
        }

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider secodaryCommandSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;
        private IElementsSubViewStateProvider backAnchorSubViewStateProvider;

        private readonly ButtonElementHandler<TElm, TMgr, TArg> secodaryCommandSubViewHandler = new();

        public Builder ShowTemplate(
            IReadOnlyList<TElm> list, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                secodaryCommandSubViewStateProvider?.Reset();
                captionBoxSubViewStateProvider?.Reset();
                backAnchorSubViewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            OriginalList.Clear();
            OriginalList.AddRange(list);

            if (TryShowSubViews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubViews(TMgr manager, TArg arg)
        {
            manager
                .GetSubView(SecodaryCommandSubViewName)
                .Show(List, secodaryCommandSubViewHandler, manager, arg, ref secodaryCommandSubViewStateProvider);

            if (Title != null)
            {
                manager
                    .GetSubView(CaptionBoxSubViewName)
                    .Show(TitleSingle, ElementToStringHandler.Instance, manager, arg, ref captionBoxSubViewStateProvider);
            }

            if (BackAnchorSubViewName != null)
            {
                manager
                    .GetSubView(BackAnchorSubViewName)
                    .Show(BackAnchorList, SelectOptionHandler.Instance, manager, arg, ref backAnchorSubViewStateProvider);
            }
        }

        public void HideTemplate(TMgr manager, bool back)
        {
            manager.GetSubView(SecodaryCommandSubViewName).Hide(back);
            if (Title != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
            if (BackAnchorSubViewName != null) { manager.GetSubView(BackAnchorSubViewName).Hide(back); }
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
                AssertNotBuilded();

                if (parent.secodaryCommandSubViewHandler.GetName != null) { Debug.LogWarning($"{nameof(NameFrom)} が多重購読されました。"); }

                parent.secodaryCommandSubViewHandler.GetName += nameFrom;
                return this;
            }

            public Builder StyleFrom(GetElementStyle<TElm, TMgr, TArg> styleFrom)
            {
                AssertNotBuilded();

                if (parent.secodaryCommandSubViewHandler.GetStyle != null) { Debug.LogWarning($"{nameof(StyleFrom)} が多重購読されました。"); }

                parent.secodaryCommandSubViewHandler.GetStyle += styleFrom;
                return this;
            }

            public Builder OnClick(HandleClickElement<TElm, TMgr, TArg> onClick)
            {
                AssertNotBuilded();

                parent.secodaryCommandSubViewHandler.HandleClick += onClick;
                return this;
            }
        }
    }
}
