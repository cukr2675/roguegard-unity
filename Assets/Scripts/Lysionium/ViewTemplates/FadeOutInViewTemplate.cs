using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// 画面のフェードアウト/フェードインを扱う ViewTemplate
    /// </summary>
    public class FadeOutInViewTemplate<TMgr, TArg> : ViewTemplate<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string FadeMaskSubviewName { get; set; } = StandardSubviewTable.FadeMaskName;

        private object prevViewStateHolder;
        private IElementsSubviewStateProvider fadeMaskSubviewStateProvider;
        private event HandleClickElement<TMgr, TArg> HandleFadeOut;
        private event HandleClickElement<TMgr, TArg> HandleFadeIn;

        private readonly List<object> widgetOptions = new();
        private readonly HandleEndAnimation onFadeOutAnimation;
        private readonly HandleEndAnimation onFadeInAnimation;

        public FadeOutInViewTemplate()
        {
            onFadeOutAnimation = (manager, arg) =>
            {
                if (LUIAssert.Type<TMgr>(manager, out var tMgr)) return;
                if (LUIAssert.Type<TArg>(arg, out var tArg)) return;

                HandleFadeOut?.Invoke(tMgr, tArg);
            };

            onFadeInAnimation = (manager, arg) =>
            {
                if (LUIAssert.Type<TMgr>(manager, out var tMgr)) return;
                if (LUIAssert.Type<TArg>(arg, out var tArg)) return;

                HandleFadeIn?.Invoke(tMgr, tArg);
            };
        }

        public Builder FadeOut(TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                fadeMaskSubviewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            if (TryShowSubviews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
        {
            manager
                .GetSubview(FadeMaskSubviewName)
                .Show(widgetOptions, ElementToStringHandler.Instance, manager, arg, ref fadeMaskSubviewStateProvider, onFadeOutAnimation);
        }

        public void FadeIn(TMgr manager, bool back)
        {
            manager.GetSubview(FadeMaskSubviewName).Hide(back, onFadeInAnimation);
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
                AssertNotBuilt();

                parent.widgetOptions.Add(widgetOption);
                return this;
            }

            public Builder OnFadeOutCompleted(HandleClickElement<TMgr, TArg> onFadeOut)
            {
                AssertNotBuilt();

                parent.HandleFadeOut += onFadeOut;
                return this;
            }

            public Builder OnFadeInCompleted(HandleClickElement<TMgr, TArg> onFadeIn)
            {
                AssertNotBuilt();

                parent.HandleFadeIn += onFadeIn;
                return this;
            }
        }
    }
}
