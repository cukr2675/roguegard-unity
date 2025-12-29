namespace Lysionium
{
    /// <inheritdoc/>
    public class ChoicesScreen<TMgr> : ChoicesScreen<TMgr, IListuiArg>
        where TMgr : IListuiManager
    {
        public ChoicesScreen(
            string message, bool isIncremental = true,
            System.Func<TMgr, IMessageBoxSubview> speechBoxSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> choicesSubviewSelector = null)
            : base(message, isIncremental, speechBoxSubviewSelector, choicesSubviewSelector)
        { }

        public ChoicesScreen(
            System.Func<TMgr, IListuiArg, string> getMessage, bool isIncremental = true,
            System.Func<TMgr, IMessageBoxSubview> speechBoxSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> choicesSubviewSelector = null)
            : base(getMessage, isIncremental, speechBoxSubviewSelector, choicesSubviewSelector)
        { }
    }

    public class ChoicesScreen<TMgr, TArg> : IListuiScreen<TMgr, TArg>, ISelectOptionListBuilder<TMgr, TArg, ChoicesScreen<TMgr, TArg>>
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        private readonly System.Func<TMgr, TArg, string> getMessage;
        private readonly SelectOptionList<TMgr, TArg> selectOptions = new();
        private readonly SpeechBoxViewData<TMgr, TArg> view;

        public bool IsIncremental { get; }

        public ChoicesScreen(
            string message, bool isIncremental = true,
            System.Func<TMgr, IMessageBoxSubview> speechBoxSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> choicesSubviewSelector = null)
        {
            getMessage = delegate { return message; };
            IsIncremental = isIncremental;

            view = new()
            {
            };
            if (speechBoxSubviewSelector != null) { view.SpeechBoxSubviewSelector = speechBoxSubviewSelector; }
            if (choicesSubviewSelector != null) { view.ChoicesSubviewSelector = choicesSubviewSelector; }
        }

        public ChoicesScreen(
            System.Func<TMgr, TArg, string> getMessage, bool isIncremental = true,
            System.Func<TMgr, IMessageBoxSubview> speechBoxSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> choicesSubviewSelector = null)
        {
            this.getMessage = getMessage;
            IsIncremental = isIncremental;

            view = new()
            {
            };
            if (speechBoxSubviewSelector != null) { view.SpeechBoxSubviewSelector = speechBoxSubviewSelector; }
            if (choicesSubviewSelector != null) { view.ChoicesSubviewSelector = choicesSubviewSelector; }
        }

        public ChoicesScreen<TMgr, TArg> Option(ISelectOption<TMgr, TArg> option)
        {
            selectOptions.Option(option);
            return this;
        }

        public void OpenScreen(TMgr manager, TArg arg)
        {
            var message = getMessage(manager, arg);

            view.Show(message, manager, arg)
                ?
                .OptionRange(selectOptions)

                .Build();
        }

        public void CloseScreenView(TMgr manager, bool back)
        {
            if (IsIncremental) { view.Hide(manager, back); }
            else { manager.HideAll(back); }
        }

        ChoicesScreen<TMgr, TArg> ISelectOptionListBuilder<TMgr, TArg, ChoicesScreen<TMgr, TArg>>.Option() => this;
    }
}
