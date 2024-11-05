using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class ScrollViewTemplate<TElm, TMgr, TArg> : ListViewTemplate<TElm, TMgr, TArg>
        where TElm : class
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string ScrollSubViewName { get; set; } = StandardSubViewTable.ScrollName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;
        public string BackAnchorSubViewName { get; set; } = StandardSubViewTable.BackAnchorName;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider scrollSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;
        private IElementsSubViewStateProvider backAnchorSubViewStateProvider;

        private readonly ButtonElementHandler<TElm, TMgr, TArg> scrollSubViewHandler = new();

        public Builder ShowTemplate(
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
            OriginalList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                OriginalList.Add(list[i]);
            }

            if (TryShowSubViews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubViews(TMgr manager, TArg arg)
        {
            manager
                .GetSubView(ScrollSubViewName)
                .Show(List, scrollSubViewHandler, manager, arg, ref scrollSubViewStateProvider);

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
        }

        public void HideTemplate(TMgr manager, bool back)
        {
            manager.GetSubView(ScrollSubViewName).Hide(back);
            if (Title != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
            if (BackAnchorList != null) { manager.GetSubView(BackAnchorSubViewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<Builder>
        {
            private readonly ScrollViewTemplate<TElm, TMgr, TArg> parent;

            public Builder(ScrollViewTemplate<TElm, TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder ElementNameFrom(GetElementName<TElm, TMgr, TArg> method)
            {
                AssertNotBuilded();

                parent.scrollSubViewHandler.GetName = method;
                return this;
            }

            public Builder OnClickElement(HandleClickElement<TElm, TMgr, TArg> method)
            {
                AssertNotBuilded();

                parent.scrollSubViewHandler.HandleClick = method;
                return this;
            }
        }
    }
}
