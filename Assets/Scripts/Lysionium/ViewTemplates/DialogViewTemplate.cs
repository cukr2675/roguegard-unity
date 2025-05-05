using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// テキストと項目を表示する ViewTemplate
    /// </summary>
    public class DialogViewTemplate<TMgr, TArg> : ListViewTemplate<object, TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string DialogSubviewName { get; set; } = StandardSubviewTable.DialogName;
        public string CaptionBoxSubviewName { get; set; } = StandardSubviewTable.CaptionBoxName;
        public string BackAnchorSubviewName { get; set; } = StandardSubviewTable.BackAnchorName;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        private object prevViewStateHolder;
        private IElementsSubviewStateProvider dialogSubviewStateProvider;
        private IElementsSubviewStateProvider captionBoxSubviewStateProvider;
        private IElementsSubviewStateProvider backAnchorSubviewStateProvider;

        private string message;
        private event HandleClickElement<string, TMgr, TArg> handleClickLink;

        public Builder ShowTemplate(string message, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (message == null) throw new System.ArgumentNullException(nameof(message));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                dialogSubviewStateProvider?.Reset();
                captionBoxSubviewStateProvider?.Reset();
                backAnchorSubviewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            // スクロールのビューを表示
            this.message = message;

            if (TryShowSubviews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
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
                .GetSubview(DialogSubviewName)
                .Show(List, SelectOptionHandler.Instance, manager, arg, ref dialogSubviewStateProvider);

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
            manager.GetSubview(DialogSubviewName).Hide(back);
            if (Title != null) { manager.GetSubview(CaptionBoxSubviewName).Hide(back); }
            if (BackAnchorSubviewName != null) { manager.GetSubview(BackAnchorSubviewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<Builder>
        {
            private readonly DialogViewTemplate<TMgr, TArg> parent;

            public Builder(DialogViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder Option(string name, HandleClickElement<TMgr, TArg> onClick, string style = null)
            {
                AssertNotBuilt();

                Tail(SelectOption.Create(name, onClick, style));
                return this;
            }

            public Builder StackOptions(params (string, HandleClickElement<TMgr, TArg>)[] selectOptions)
            {
                AssertNotBuilt();

                var stack = new List<object>();
                foreach (var selectOption in selectOptions)
                {
                    stack.Add(SelectOption.Create(selectOption.Item1, selectOption.Item2));
                }
                Tail(stack);
                return this;
            }

            public Builder OnClickLink(HandleClickElement<string, TMgr, TArg> onClickLink)
            {
                AssertNotBuilt();

                parent.handleClickLink += onClickLink;
                return this;
            }
        }
    }
}
