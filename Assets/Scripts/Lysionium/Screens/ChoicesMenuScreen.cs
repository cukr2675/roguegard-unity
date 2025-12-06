using System.Collections.Generic;

namespace Lysionium
{
    public class ChoicesMenuScreen<TMgr, TArg> : MenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly System.Func<TMgr, TArg, string> getMessage;
        private readonly List<ISelectOption> selectOptions = new();
        private readonly SpeechBoxViewData<TMgr, TArg> view;

        public override bool IsIncremental { get; }

        [System.Obsolete]
        public ChoicesMenuScreen(
            string message, bool isIncremental, string speechBoxSubviewName, string choicesSubviewName = null)
        {
            getMessage = delegate { return message; };
            IsIncremental = isIncremental;

            view = new()
            {
            };
            if (speechBoxSubviewName != null) { view.SpeechBoxSubviewName = speechBoxSubviewName; }
            if (choicesSubviewName != null) { view.ChoicesSubviewName = choicesSubviewName; }
        }

        [System.Obsolete]
        public ChoicesMenuScreen(
            System.Func<TMgr, TArg, string> getMessage, bool isIncremental, string speechBoxSubviewName, string choicesSubviewName = null)
        {
            this.getMessage = getMessage;
            IsIncremental = isIncremental;

            view = new()
            {
            };
            if (speechBoxSubviewName != null) { view.SpeechBoxSubviewName = speechBoxSubviewName; }
            if (choicesSubviewName != null) { view.ChoicesSubviewName = choicesSubviewName; }
        }

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

        public ChoicesMenuScreen<TMgr, TArg> Option(string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
        {
            selectOptions.Add(SelectOption.Create(name, onClick, style));
            return this;
        }

        public ChoicesMenuScreen<TMgr, TArg> Back(string name = null)
        {
            if (name == null)
            {
                selectOptions.Add(BackSelectOption.Instance);
            }
            else
            {
                selectOptions.Add(BackSelectOption.Create<TMgr, TArg>(name));
            }
            return this;
        }

        public override void OpenScreen(in TMgr manager, in TArg arg)
        {
            var message = getMessage(manager, arg);

            view.Show(message, manager, arg)
                ?
                .TailRange(selectOptions)

                .Build();
        }

        public override void CloseScreenView(TMgr manager, bool back)
        {
            if (IsIncremental) { view.Hide(manager, back); }
            else { base.CloseScreenView(manager, back); }
        }
    }
}
