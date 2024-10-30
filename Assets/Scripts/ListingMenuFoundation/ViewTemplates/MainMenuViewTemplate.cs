using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class MainMenuViewTemplate<TMgr, TArg> : ListViewTemplate<IListMenuSelectOption, TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string PrimaryCommandSubViewName { get; set; } = StandardSubViewTable.PrimaryCommandName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;
        public string BackAnchorSubViewName { get; set; } = null;
        public List<IListMenuSelectOption> BackAnchorList { get; set; } = new() { ExitListMenuSelectOption.Instance };

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider primaryCommandSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;
        private IElementsSubViewStateProvider backAnchorSubViewStateProvider;

        public Builder ShowTemplate(TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                primaryCommandSubViewStateProvider?.Reset();
                captionBoxSubViewStateProvider?.Reset();
                backAnchorSubViewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            if (TryShowSubViews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubViews(TMgr manager, TArg arg)
        {
            manager
                .GetSubView(PrimaryCommandSubViewName)
                .Show(List, SelectOptionHandler.Instance, manager, arg, ref primaryCommandSubViewStateProvider);

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
            manager.GetSubView(PrimaryCommandSubViewName).Hide(back);
            if (Title != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
            if (BackAnchorSubViewName != null) { manager.GetSubView(BackAnchorSubViewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<Builder>
        {
            private readonly MainMenuViewTemplate<TMgr, TArg> parent;

            public Builder(MainMenuViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder Option(string name, HandleClickElement<TMgr, TArg> handleClick)
            {
                return Append(ListMenuSelectOption.Create(name, handleClick));
            }

            public Builder Exit()
            {
                return Append(ExitListMenuSelectOption.Instance);
            }
        }
    }
}
