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

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider primaryCommandSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;

        public FluentBuilder Show(TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                primaryCommandSubViewStateProvider?.Reset();
                captionBoxSubViewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            if (TryShowSubViews(manager, arg)) return null;
            else return new FluentBuilder(this, manager, arg);
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
        }

        public void HideSubViews(TMgr manager, bool back)
        {
            manager.GetSubView(PrimaryCommandSubViewName).Hide(back);
            if (Title != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
        }

        public class FluentBuilder : ListFluentBuilder<FluentBuilder>
        {
            private readonly MainMenuViewTemplate<TMgr, TArg> parent;

            public FluentBuilder(MainMenuViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public FluentBuilder Option(string name, ListMenuSelectOption<TMgr, TArg>.HandleClickAction handleClick)
            {
                return Append(ListMenuSelectOption.Create(name, handleClick));
            }

            public FluentBuilder Option(string name, IMenuScreen menuScreen)
            {
                return Append(ListMenuSelectOption.Create(name, (TMgr manager, TArg arg) => manager.PushMenuScreen(menuScreen, arg)));
            }

            public FluentBuilder Exit()
            {
                return Append(ExitListMenuSelectOption.Instance);
            }
        }
    }
}
