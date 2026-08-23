namespace Lysionium
{
    /// <summary>
    /// テキストと項目を表示する ViewData
    /// </summary>
    public class DialogViewData<TMgr> : ListViewData<object, TMgr>
        where TMgr : IListuiManager
    {
        public System.Func<TMgr, IListHandlerSubview> DialogSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.Dialog;
        public System.Func<TMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<TMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.BackAnchor;
        public SelectOptionList<TMgr> BackAnchorList { get; set; } = new(_ => _.BackIfReflectable());

        private object prevViewStateHolder;
        private ISubviewStateProvider dialogSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        private string message;
        private event SubmitItemHandler<string, TMgr> ClickLink;

        public Builder Show(string message, TMgr manager, object viewStateHolder = null)
        {
            if (message == null) throw new System.ArgumentNullException(nameof(message));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder) { ResetSubviewStateProviders(); }
            prevViewStateHolder = viewStateHolder;

            this.message = message;

            if (TryShowSubviews(manager)) return null;
            else return new Builder(this, manager);
        }

        protected virtual void ResetSubviewStateProviders()
        {
            dialogSubviewStateProvider?.Reset();
            captionBoxSubviewStateProvider?.Reset();
            backAnchorSubviewStateProvider?.Reset();
        }

        protected override void ShowSubviews(TMgr manager)
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
                List, SelectOptionViewItemHandler<TMgr>.Instance, manager, ref dialogSubviewStateProvider, onHide: OnHide);

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    Title, manager, ref captionBoxSubviewStateProvider);
            }

            BackAnchorSubviewSelector?.Invoke(manager)?.Show(
                BackAnchorList, manager, ref backAnchorSubviewStateProvider);
        }

        public virtual void Hide(TMgr manager, bool back)
        {
            DialogSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder : BaseListBuilder<DialogViewData<TMgr>, Builder>
        {
            public Builder(DialogViewData<TMgr> parent, TMgr manager)
                : base(parent, manager)
            {
            }

            //public Builder Option(string name, ClickOptionHandler<TMgr> onClick, string style = null)
            //{
            //    AssertNotBuilt();

            //    Tail.Option(SelectOption.Create(name, onClick, style));
            //    return this;
            //}

            //public Builder OptionStack(params (string, ClickOptionHandler<TMgr>)[] selectOptions)
            //{
            //    AssertNotBuilt();

            //    var stack = new (string, object)[selectOptions.Length];
            //    for (int i = 0; i < selectOptions.Length; i++)
            //    {
            //        stack[i] = ("1*", SelectOption.Create(selectOptions[i].Item1, selectOptions[i].Item2));
            //    }
            //    Tail.Append(StackWidgetOption.Create(stack));
            //    return this;
            //}

            public Builder OnClickLink(SubmitItemHandler<string, TMgr> onClickLink)
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
