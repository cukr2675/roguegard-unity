using System.Collections.Generic;

namespace Lysionium
{
    public class ChoicesMenuScreen<TMgr, TArg> : MenuScreen<TMgr, TArg>, ISelectOptionListBuilder<TMgr, TArg, ChoicesMenuScreen<TMgr, TArg>>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly System.Func<TMgr, TArg, string> getMessage;
        private readonly List<ISelectOption> selectOptions = new();
        private readonly SpeechBoxViewData<TMgr, TArg> view;

        public override bool IsIncremental { get; }

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

        public ChoicesMenuScreen<TMgr, TArg> Option(ISelectOption option)
        {
            selectOptions.Add(option);
            return this;
        }

        public override void OpenScreen(TMgr manager, TArg arg)
        {
            var message = getMessage(manager, arg);

            view.Show(message, manager, arg)
                ?
                .OptionRange(selectOptions)

                .Build();
        }

        public override void CloseScreenView(TMgr manager, bool back)
        {
            if (IsIncremental) { view.Hide(manager, back); }
            else { base.CloseScreenView(manager, back); }
        }
    }
}
