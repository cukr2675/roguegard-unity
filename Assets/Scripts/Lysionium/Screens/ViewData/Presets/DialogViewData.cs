using System.Collections.Generic;

namespace Lysionium
{
    /// <inheritdoc/>
    public class DialogViewData<TMgr> : DialogViewData<TMgr, IListuiArg>
        where TMgr : IListuiManager
    { }

    /// <summary>
    /// テキストと項目を表示する ViewData
    /// </summary>
    public class DialogViewData<TMgr, TArg> : ListViewData<object, TMgr, TArg>
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        public System.Func<TMgr, IListHandlerSubview> DialogSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.Dialog;
        public System.Func<TMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<TMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.BackAnchor;
        public SelectOptionList<TMgr, TArg> BackAnchorList { get; set; } = new(_ => _.BackIfReflectable());

        private object prevViewStateHolder;
        private ISubviewStateProvider dialogSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        private string message;
        private event ClickItemHandler<string, TMgr, TArg> ClickLink;

        public Builder Show(string message, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (message == null) throw new System.ArgumentNullException(nameof(message));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder) { ResetSubviewStateProviders(); }
            prevViewStateHolder = viewStateHolder;

            this.message = message;

            if (TryShowSubviews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected virtual void ResetSubviewStateProviders()
        {
            dialogSubviewStateProvider?.Reset();
            captionBoxSubviewStateProvider?.Reset();
            backAnchorSubviewStateProvider?.Reset();
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
        {
            OriginalList.Clear();
            if (ClickLink != null)
            {
                OriginalList.Add(LabelWidgetOption.Create(message, ClickLink));
            }
            else
            {
                OriginalList.Add(message);
            }

            DialogSubviewSelector?.Invoke(manager)?.Show(
                List, SelectOptionViewItemHandler<TMgr, TArg>.Instance, manager, arg, ref dialogSubviewStateProvider, onHide: OnHide);

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    Title, manager, arg, ref captionBoxSubviewStateProvider);
            }

            BackAnchorSubviewSelector?.Invoke(manager)?.Show(
                BackAnchorList, manager, arg, ref backAnchorSubviewStateProvider);
        }

        public virtual void Hide(TMgr manager, bool back)
        {
            DialogSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder : BaseListBuilder<DialogViewData<TMgr, TArg>, Builder>
        {
            public Builder(DialogViewData<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            //public Builder Option(string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
            //{
            //    AssertNotBuilt();

            //    Tail(SelectOption.Create(name, onClick, style));
            //    return this;
            //}

            //public Builder OptionStack(params (string, ClickItemHandler<TMgr, TArg>)[] selectOptions)
            //{
            //    AssertNotBuilt();

            //    var stack = new (string, object)[selectOptions.Length];
            //    for (int i = 0; i < selectOptions.Length; i++)
            //    {
            //        stack[i] = ("1*", SelectOption.Create(selectOptions[i].Item1, selectOptions[i].Item2));
            //    }
            //    Tail(StackWidgetOption.Create(stack));
            //    return this;
            //}

            public Builder OnClickLink(ClickItemHandler<string, TMgr, TArg> onClickLink)
            {
                AssertNotBuilt();

                Parent.ClickLink += onClickLink;
                return this;
            }

            protected override void Unload()
            {
                base.Unload();
                Parent.ClickLink = null;
            }
        }
    }
}
