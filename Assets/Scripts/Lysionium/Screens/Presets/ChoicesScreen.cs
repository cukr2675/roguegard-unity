namespace Lysionium
{
    public class ChoicesScreen<TMgr> : IListuiScreen<TMgr>, ISelectOptionsBuilder<TMgr, ChoicesScreen<TMgr>>
        where TMgr : IListuiManager
    {
        private readonly System.Func<TMgr, string> getMessage;
        private readonly SelectOptionList<TMgr> selectOptions = new();
        private readonly SpeechBoxViewData<TMgr> view;

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
            System.Func<TMgr, string> getMessage, bool isIncremental = true,
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

        public ChoicesScreen<TMgr> Option(ISelectOption<TMgr> option)
        {
            selectOptions.Option(option);
            return this;
        }

        public void OpenScreen(TMgr manager)
        {
            var message = getMessage(manager);

            view.Show(message, manager)
                ?
                .OptionRange(selectOptions)

                .Build();
        }

        public void CloseScreenView(TMgr manager, bool back)
        {
            if (IsIncremental) { view.Hide(manager, back); }
            else { manager.HideAll(back); }
        }

        ChoicesScreen<TMgr> ISelectOptionsBuilder<TMgr, ChoicesScreen<TMgr>>.Option() => this;
    }

    public class ChoicesScreen<TMgr, TArg> : IListuiScreen<TMgr, TArg>, ISelectOptionsBuilder<TMgr, TArg, ChoicesScreen<TMgr, TArg>>
        where TMgr : IListuiManager
    {
        private readonly System.Func<TMgr, TArg, string> getMessage;
        private readonly SelectOptionList<TMgr> selectOptions = new();
        private readonly SpeechBoxViewData<TMgr> view;

        private TArg arg;

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

        public ChoicesScreen<TMgr, TArg> Option(ISelectOption<TMgr> option)
        {
            selectOptions.Option(option);
            return this;
        }

        public ChoicesScreen<TMgr, TArg> Option(ISelectOption<TMgr, TArg> option)
        {
            selectOptions.Option(
                SelectOption.Create<TMgr>(
                    m => option.GetName(m, arg),
                    m => option.Click(m, arg),
                    m => option.GetStyle(m, arg)));
            return this;
        }

        public void OpenScreen(TMgr manager, TArg arg)
        {
            this.arg = arg;
            var message = getMessage(manager, arg);

            view.Show(message, manager, arg)
                ?
                .OptionRange(selectOptions)

                .Build();
        }

        public void CloseScreenView(TMgr manager, bool back, TArg arg)
        {
            if (IsIncremental) { view.Hide(manager, back); }
            else { manager.HideAll(back); }
        }

        ChoicesScreen<TMgr, TArg> ISelectOptionsBuilder<TMgr, ChoicesScreen<TMgr, TArg>>.Option() => this;
    }
}
