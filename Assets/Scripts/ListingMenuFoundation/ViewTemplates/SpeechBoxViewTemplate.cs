using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class SpeechBoxViewTemplate<TMgr, TArg> : ListViewTemplate<IListMenuSelectOption, TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string MenuName { get; set; }
        public string SpeechBoxSubViewName { get; set; } = StandardSubViewTable.SpeechBoxName;
        public string ChoicesSubViewName { get; set; } = StandardSubViewTable.ChoicesName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider messageBoxSubViewStateProvider;
        private IElementsSubViewStateProvider choicesSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;

        private readonly string[] message = new string[1];
        private readonly List<object> captionBoxList = new();

        public FluentBuilder Show(string message, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (message == null) throw new System.ArgumentNullException(nameof(message));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                messageBoxSubViewStateProvider?.Reset();
                choicesSubViewStateProvider?.Reset();
                captionBoxSubViewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            // メッセージボックスのビューを表示
            this.message[0] = message;

            // メニュー名を表示
            captionBoxList.Clear();
            if (MenuName != null) { captionBoxList.Add(MenuName); }

            if (TryShowSubViews(manager, arg)) return null;
            else return new FluentBuilder(this, manager, arg);
        }

        protected override void ShowSubViews(TMgr manager, TArg arg)
        {
            if (LMFAssert.Type<MessageBoxSubView>(manager.GetSubView(SpeechBoxSubViewName), out var speechBoxSubView)) return;
            speechBoxSubView.Show(message, ElementToStringHandler.Instance, manager, arg, ref messageBoxSubViewStateProvider);

            manager
                .GetSubView(ChoicesSubViewName)
                .SetParameters(List, SelectOptionHandler.Instance, manager, arg, ref choicesSubViewStateProvider);
            speechBoxSubView.ShowScheduledAfterCompletion(ChoicesSubViewName);

            if (MenuName != null)
            {
                manager
                    .GetSubView(CaptionBoxSubViewName)
                    .Show(captionBoxList, ElementToStringHandler.Instance, manager, arg, ref captionBoxSubViewStateProvider);
            }
        }

        public void HideSubViews(TMgr manager, bool back)
        {
            manager.GetSubView(SpeechBoxSubViewName).Hide(back);
            manager.GetSubView(ChoicesSubViewName).Hide(back);
            if (MenuName != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
        }

        public class FluentBuilder : ListFluentBuilder<FluentBuilder>
        {
            private readonly SpeechBoxViewTemplate<TMgr, TArg> parent;

            public FluentBuilder(SpeechBoxViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public FluentBuilder AppendSelectOption(string name, ListMenuSelectOption<TMgr, TArg>.HandleClickAction handleClick)
            {
                AssertNotBuilded();

                Append(ListMenuSelectOption.Create(name, handleClick));
                return this;
            }

            public FluentBuilder Exit()
            {
                AssertNotBuilded();

                Append(ExitListMenuSelectOption.Instance);
                return this;
            }
        }
    }
}
