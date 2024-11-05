using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class DialogViewTemplate<TMgr, TArg> : ListViewTemplate<object, TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string DialogSubViewName { get; set; } = StandardSubViewTable.DialogName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;
        public string BackAnchorSubViewName { get; set; } = null;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider dialogSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;
        private IElementsSubViewStateProvider backAnchorSubViewStateProvider;

        private string message;
        private event HandleClickElement<string, TMgr, TArg> handleClickLink;

        public Builder ShowTemplate(string message, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (message == null) throw new System.ArgumentNullException(nameof(message));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                dialogSubViewStateProvider?.Reset();
                captionBoxSubViewStateProvider?.Reset();
                backAnchorSubViewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            // スクロールのビューを表示
            this.message = message;

            if (TryShowSubViews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubViews(TMgr manager, TArg arg)
        {
            OriginalList.Clear();
            if (handleClickLink != null)
            {
                OriginalList.Add(LabelViewWidget.CreateOption(message, handleClickLink));
            }
            else
            {
                OriginalList.Add(message);
            }

            manager
                .GetSubView(DialogSubViewName)
                .Show(List, SelectOptionHandler.Instance, manager, arg, ref dialogSubViewStateProvider);

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
            manager.GetSubView(DialogSubViewName).Hide(back);
            if (Title != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<Builder>
        {
            private readonly DialogViewTemplate<TMgr, TArg> parent;

            public Builder(DialogViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder AppendSelectOption(string name, HandleClickElement<TMgr, TArg> handleClick)
            {
                AssertNotBuilded();

                Append(SelectOption.Create(name, handleClick));
                return this;
            }

            public Builder AppendSelectOptions(params (string, HandleClickElement<TMgr, TArg>)[] selectOptions)
            {
                AssertNotBuilded();

                var stack = new List<object>();
                foreach (var selectOption in selectOptions)
                {
                    stack.Add(SelectOption.Create(selectOption.Item1, selectOption.Item2));
                }
                Append(stack);
                return this;
            }

            public Builder OnClickLink(HandleClickElement<string, TMgr, TArg> handleClickLink)
            {
                AssertNotBuilded();

                parent.handleClickLink += handleClickLink;
                return this;
            }
        }
    }
}
