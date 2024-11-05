using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class VariableWidgetsViewTemplate<TMgr, TArg> : ListViewTemplate<object, TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string WidgetsSubViewName { get; set; } = StandardSubViewTable.WidgetsName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;
        public string BackAnchorSubViewName { get; set; } = StandardSubViewTable.BackAnchorName;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider primaryCommandSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;
        private IElementsSubViewStateProvider backAnchorSubViewStateProvider;

        public Builder ShowTemplate(IReadOnlyList<object> widgetOptions, TMgr manager, TArg arg, object viewStateHolder = null)
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

            OriginalList.Clear();
            for (int i = 0; i < widgetOptions.Count; i++)
            {
                OriginalList.Add(widgetOptions[i]);
            }

            if (TryShowSubViews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubViews(TMgr manager, TArg arg)
        {
            manager
                .GetSubView(WidgetsSubViewName)
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
            manager.GetSubView(WidgetsSubViewName).Hide(back);
            if (Title != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
            if (BackAnchorSubViewName != null) { manager.GetSubView(BackAnchorSubViewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<Builder>
        {
            private readonly VariableWidgetsViewTemplate<TMgr, TArg> parent;

            public Builder(VariableWidgetsViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }
        }
    }
}
