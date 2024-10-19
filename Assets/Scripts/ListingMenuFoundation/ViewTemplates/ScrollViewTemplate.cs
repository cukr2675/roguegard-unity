using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class ScrollViewTemplate<TElm, TMgr, TArg> : ViewTemplate<TMgr, TArg>
        where TElm : class
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string ScrollSubViewName { get; set; } = StandardSubViewTable.ScrollName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;
        public string BackAnchorSubViewName { get; set; } = StandardSubViewTable.BackAnchorName;
        public IReadOnlyList<IListMenuSelectOption> BackAnchorList { get; set; }

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider scrollSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;
        private IElementsSubViewStateProvider backAnchorSubViewStateProvider;
        private IReadOnlyList<TElm> list;

        private readonly ButtonElementHandler<TElm, TMgr, TArg> scrollSubViewHandler = new();

        public FluentBuilder Show(
            IReadOnlyList<TElm> list, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                scrollSubViewStateProvider?.Reset();
                captionBoxSubViewStateProvider?.Reset();
                backAnchorSubViewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            // スクロールのビューを表示
            this.list = list;

            if (TryShowSubViews(manager, arg)) return null;
            else return new FluentBuilder(this, manager, arg);
        }

        protected override void ShowSubViews(TMgr manager, TArg arg)
        {
            manager
                .GetSubView(ScrollSubViewName)
                .Show(list, scrollSubViewHandler, manager, arg, ref scrollSubViewStateProvider);

            if (Title != null)
            {
                manager
                    .GetSubView(CaptionBoxSubViewName)
                    .Show(TitleSingle, ElementToStringHandler.Instance, manager, arg, ref captionBoxSubViewStateProvider);
            }

            if (BackAnchorList != null)
            {
                manager
                    .GetSubView(BackAnchorSubViewName)
                    .Show(BackAnchorList, SelectOptionHandler.Instance, manager, arg, ref backAnchorSubViewStateProvider);
            }
            else
            {
                ExitListMenuSelectOption.ShowBackAnchorExit(manager);
            }
        }

        public void HideSubViews(TMgr manager, bool back)
        {
            manager.GetSubView(ScrollSubViewName).Hide(back);
            if (Title != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
            manager.GetSubView(BackAnchorSubViewName).Hide(back);
        }

        public class FluentBuilder : BaseFluentBuilder
        {
            private readonly ScrollViewTemplate<TElm, TMgr, TArg> parent;

            public FluentBuilder(ScrollViewTemplate<TElm, TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public FluentBuilder ElementNameGetter(ButtonElementHandler<TElm, TMgr, TArg>.GetNameFunc method)
            {
                AssertNotBuilded();

                parent.scrollSubViewHandler.GetName = method;
                return this;
            }

            public FluentBuilder OnClickElement(ButtonElementHandler<TElm, TMgr, TArg>.HandleClickAction method)
            {
                AssertNotBuilded();

                parent.scrollSubViewHandler.HandleClick = method;
                return this;
            }
        }
    }
}
