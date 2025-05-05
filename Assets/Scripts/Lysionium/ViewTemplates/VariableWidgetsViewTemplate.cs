using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// 項目数が可変のウィジェットメニュー向け ViewTemplate
    /// </summary>
    public class VariableWidgetsViewTemplate<TMgr, TArg> : ListViewTemplate<object, TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string WidgetsSubviewName { get; set; } = StandardSubviewTable.WidgetsName;
        public string CaptionBoxSubviewName { get; set; } = StandardSubviewTable.CaptionBoxName;
        public string BackAnchorSubviewName { get; set; } = StandardSubviewTable.BackAnchorName;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        private object prevViewStateHolder;
        private IElementsSubviewStateProvider primaryCommandSubviewStateProvider;
        private IElementsSubviewStateProvider captionBoxSubviewStateProvider;
        private IElementsSubviewStateProvider backAnchorSubviewStateProvider;

        public Builder ShowTemplate(IReadOnlyList<object> widgetOptions, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                primaryCommandSubviewStateProvider?.Reset();
                captionBoxSubviewStateProvider?.Reset();
                backAnchorSubviewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            OriginalList.Clear();
            for (int i = 0; i < widgetOptions.Count; i++)
            {
                OriginalList.Add(widgetOptions[i]);
            }

            if (TryShowSubviews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
        {
            manager
                .GetSubview(WidgetsSubviewName)
                .Show(List, SelectOptionHandler.Instance, manager, arg, ref primaryCommandSubviewStateProvider);

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
            manager.GetSubview(WidgetsSubviewName).Hide(back);
            if (Title != null) { manager.GetSubview(CaptionBoxSubviewName).Hide(back); }
            if (BackAnchorSubviewName != null) { manager.GetSubview(BackAnchorSubviewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<Builder>
        {
            private readonly VariableWidgetsViewTemplate<TMgr, TArg> parent;

            public Builder(VariableWidgetsViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder HeadStack(params object[] elements)
            {
                return Head(elements);
            }

            public Builder TailStack(params object[] elements)
            {
                return Tail(elements);
            }
        }
    }
}
