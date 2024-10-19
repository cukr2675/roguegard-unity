using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class CommandListViewTemplate<TElm, TMgr, TArg> : ListViewTemplate<TElm, TMgr, TArg>
        where TElm : class
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string SecodaryCommandSubViewName { get; set; } = StandardSubViewTable.SecondaryCommandName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider scrollSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;

        private readonly ButtonElementHandler<TElm, TMgr, TArg> scrollSubViewHandler = new();

        public FluentBuilder Show(
            IReadOnlyList<TElm> list, TMgr manager, TArg arg, object viewStateHolder = null, IReadOnlyList<object> backAnchorList = null)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // �K�v�ɉ����ăX�N���[���ʒu�����Z�b�g
            if (viewStateHolder != prevViewStateHolder)
            {
                scrollSubViewStateProvider?.Reset();
                captionBoxSubViewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            OriginalList.Clear();
            OriginalList.AddRange(list);

            if (TryShowSubViews(manager, arg)) return null;
            else return new FluentBuilder(this, manager, arg);
        }

        protected override void ShowSubViews(TMgr manager, TArg arg)
        {
            manager
                .GetSubView(SecodaryCommandSubViewName)
                .Show(List, scrollSubViewHandler, manager, arg, ref scrollSubViewStateProvider);

            if (Title != null)
            {
                manager
                    .GetSubView(CaptionBoxSubViewName)
                    .Show(TitleSingle, ElementToStringHandler.Instance, manager, arg, ref captionBoxSubViewStateProvider);
            }
        }

        public void HideSubViews(TMgr manager, bool back)
        {
            manager.GetSubView(SecodaryCommandSubViewName).Hide(back);
            if (Title != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
        }

        public class FluentBuilder : BaseFluentBuilder
        {
            private readonly CommandListViewTemplate<TElm, TMgr, TArg> parent;

            public FluentBuilder(CommandListViewTemplate<TElm, TMgr, TArg> parent, TMgr manager, TArg arg)
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