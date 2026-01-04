using System.Collections.Generic;

namespace Lysionium
{
    /// <inheritdoc/>
    public class FadeOutInViewData<TMgr> : FadeOutInViewData<TMgr, IListuiArg>
        where TMgr : IListuiManager
    { }

    /// <summary>
    /// 画面のフェードアウト/フェードインを扱う ViewData
    /// </summary>
    public class FadeOutInViewData<TMgr, TArg> : ViewData<TMgr, TArg>
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        public System.Func<TMgr, IListHandlerSubview> FadeMaskSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.FadeMask;

        private object prevViewStateHolder;
        private ISubviewStateProvider fadeMaskSubviewStateProvider;
        private event ClickItemHandler<TMgr, TArg> HandleFadeOut;
        private event ClickItemHandler<TMgr, TArg> HandleFadeIn;

        private readonly List<object> widgetOptions = new();
        private readonly ListuiEventHandler onFadeOutAnimation;
        private readonly ListuiEventHandler onFadeInAnimation;

        public FadeOutInViewData()
        {
            onFadeOutAnimation = (manager, arg) =>
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return;
                if (LuiAssert.Type<TArg>(arg, out var tArg)) return;

                HandleFadeOut?.Invoke(tMgr, tArg);
            };

            onFadeInAnimation = (manager, arg) =>
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return;
                if (LuiAssert.Type<TArg>(arg, out var tArg)) return;

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
            FadeMaskSubviewSelector?.Invoke(manager)?.Show(
                widgetOptions, SelectOptionViewItemHandler<TMgr, TArg>.Instance, manager, arg, ref fadeMaskSubviewStateProvider,
                onFadeOutAnimation, OnHide);
        }

        public void FadeIn(TMgr manager, bool back)
        {
            FadeMaskSubviewSelector?.Invoke(manager)?.Hide(back, onFadeInAnimation);
        }

        public class Builder : BaseBuilder<FadeOutInViewData<TMgr, TArg>, Builder>
        {
            public Builder(FadeOutInViewData<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder Append(object widgetOption)
            {
                AssertNotBuilt();

                Parent.widgetOptions.Add(widgetOption);
                return this;
            }

            public Builder OnFadeOutCompleted(ClickItemHandler<TMgr, TArg> onFadeOut)
            {
                AssertNotBuilt();

                Parent.HandleFadeOut += onFadeOut;
                return this;
            }

            public Builder OnFadeInCompleted(ClickItemHandler<TMgr, TArg> onFadeIn)
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
