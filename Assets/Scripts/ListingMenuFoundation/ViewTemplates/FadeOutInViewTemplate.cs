using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class FadeOutInViewTemplate<TMgr, TArg> : ViewTemplate<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string FadeMaskSubViewName { get; set; } = StandardSubViewTable.FadeMaskName;

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider fadeMaskSubViewStateProvider;
        private event HandleClickElement<TMgr, TArg> HandleFadeOut;
        private event HandleClickElement<TMgr, TArg> HandleFadeIn;

        private readonly List<object> widgetOptions = new();
        private readonly ElementsSubView.HandleEndAnimation handleFadeOutAnimation;
        private readonly ElementsSubView.HandleEndAnimation handleFadeInAnimation;

        public FadeOutInViewTemplate()
        {
            handleFadeOutAnimation = (manager, arg) =>
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr)) return;
                if (LMFAssert.Type<TArg>(arg, out var tArg)) return;

                HandleFadeOut?.Invoke(tMgr, tArg);
            };

            handleFadeInAnimation = (manager, arg) =>
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr)) return;
                if (LMFAssert.Type<TArg>(arg, out var tArg)) return;

                HandleFadeIn?.Invoke(tMgr, tArg);
            };
        }

        public Builder FadeOut(TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                fadeMaskSubViewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            if (TryShowSubViews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubViews(TMgr manager, TArg arg)
        {
            manager
                .GetSubView(FadeMaskSubViewName)
                .Show(widgetOptions, ElementToStringHandler.Instance, manager, arg, ref fadeMaskSubViewStateProvider, handleFadeOutAnimation);
        }

        public void FadeIn(TMgr manager, bool back)
        {
            manager.GetSubView(FadeMaskSubViewName).Hide(back, handleFadeInAnimation);
        }

        public class Builder : BaseBuilder<Builder>
        {
            private readonly FadeOutInViewTemplate<TMgr, TArg> parent;

            public Builder(FadeOutInViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder Append(object widgetOption)
            {
                AssertNotBuilded();

                parent.widgetOptions.Add(widgetOption);
                return this;
            }

            public Builder OnFadeOutCompleted(HandleClickElement<TMgr, TArg> handleFadeOut)
            {
                AssertNotBuilded();

                parent.HandleFadeOut += handleFadeOut;
                return this;
            }

            public Builder OnFadeInCompleted(HandleClickElement<TMgr, TArg> handleFadeIn)
            {
                AssertNotBuilded();

                parent.HandleFadeIn += handleFadeIn;
                return this;
            }
        }
    }
}
