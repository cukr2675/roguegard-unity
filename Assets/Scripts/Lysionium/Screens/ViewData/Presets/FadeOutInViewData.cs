using System.Collections.Generic;

namespace Lysionium
{
    /// <summary>
    /// 画面のフェードアウト/フェードインを扱う ViewData
    /// </summary>
    public class FadeOutInViewData<TMgr> : ViewData<TMgr>
        where TMgr : IListuiManager
    {
        public System.Func<TMgr, IListHandlerSubview> FadeMaskSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.FadeMask;

        private object prevViewStateHolder;
        private ISubviewStateProvider fadeMaskSubviewStateProvider;
        private event System.Action<TMgr> HandleFadeOut;
        private event System.Action<TMgr> HandleFadeIn;

        private readonly List<object> widgetOptions = new();
        private readonly System.Action<IListuiManager> onFadeOutAnimation;
        private readonly System.Action<IListuiManager> onFadeInAnimation;

        public FadeOutInViewData()
        {
            onFadeOutAnimation = (manager) =>
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return;

                HandleFadeOut?.Invoke(tMgr);
            };

            onFadeInAnimation = (manager) =>
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return;

                HandleFadeIn?.Invoke(tMgr);
            };
        }

        public Builder FadeOut(TMgr manager, object viewStateHolder = null)
        {
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                fadeMaskSubviewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            if (TryShowSubviews(manager)) return null;
            else return new Builder(this, manager);
        }

        protected override void ShowSubviews(TMgr manager)
        {
            FadeMaskSubviewSelector?.Invoke(manager)?.Show(
                widgetOptions, SelectOptionViewItemHandler<TMgr>.Instance, manager, ref fadeMaskSubviewStateProvider,
                onFadeOutAnimation, OnHide);
        }

        public void FadeIn(TMgr manager, bool back)
        {
            FadeMaskSubviewSelector?.Invoke(manager)?.Hide(back, onFadeInAnimation);
        }

        public class Builder : BaseBuilder<FadeOutInViewData<TMgr>, Builder>
        {
            public Builder(FadeOutInViewData<TMgr> parent, TMgr manager)
                : base(parent, manager)
            {
            }

            public Builder Append(object widgetOption)
            {
                AssertNotBuilt();

                Parent.widgetOptions.Add(widgetOption);
                return this;
            }

            public Builder OnFadeOutCompleted(System.Action<TMgr> onFadeOut)
            {
                AssertNotBuilt();

                Parent.HandleFadeOut += onFadeOut;
                return this;
            }

            public Builder OnFadeInCompleted(System.Action<TMgr> onFadeIn)
            {
                AssertNotBuilt();

                Parent.HandleFadeIn += onFadeIn;
                return this;
            }

            protected override void Unload()
            {
                base.Unload();
                Parent.HandleFadeOut = null;
                Parent.HandleFadeIn = null;
            }
        }
    }
}
