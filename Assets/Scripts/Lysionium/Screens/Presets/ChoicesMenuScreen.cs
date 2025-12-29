using System.Collections.Generic;

namespace Lysionium
{
    /// <inheritdoc/>
    public class ChoicesMenuScreen<TMgr> : ChoicesMenuScreen<TMgr, IListMenuArg>
        where TMgr : IListMenuManager
    {
        public ChoicesMenuScreen(
            string message, bool isIncremental = true,
            System.Func<TMgr, IMessageBoxSubview> speechBoxSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> choicesSubviewSelector = null)
            : base(message, isIncremental, speechBoxSubviewSelector, choicesSubviewSelector)
        { }

        public ChoicesMenuScreen(
            System.Func<TMgr, IListMenuArg, string> getMessage, bool isIncremental = true,
            System.Func<TMgr, IMessageBoxSubview> speechBoxSubviewSelector = null,
            System.Func<TMgr, IListHandlerSubview> choicesSubviewSelector = null)
            : base(getMessage, isIncremental, speechBoxSubviewSelector, choicesSubviewSelector)
        { }
    }

    public class ChoicesMenuScreen<TMgr, TArg> : IMenuScreen<TMgr, TArg>, ISelectOptionListBuilder<TMgr, TArg, ChoicesMenuScreen<TMgr, TArg>>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly System.Func<TMgr, TArg, string> getMessage;
        private readonly SelectOptionList<TMgr, TArg> selectOptions = new();
        private readonly SpeechBoxViewData<TMgr, TArg> view;

        public bool IsIncremental { get; }

        public ChoicesMenuScreen(
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

        public ChoicesMenuScreen(
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

        public ChoicesMenuScreen<TMgr, TArg> Option(ISelectOption<TMgr, TArg> option)
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

        ChoicesMenuScreen<TMgr, TArg> ISelectOptionListBuilder<TMgr, TArg, ChoicesMenuScreen<TMgr, TArg>>.Option() => this;
    }
}
