using System.Collections.Generic;

namespace Lysionium
{
    public class ChoicesMenuScreen<TMgr, TArg> : MenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly ItemNameSelector<TMgr, TArg> getMessage;
        private readonly List<ISelectOption> selectOptions = new();
        private readonly SpeechBoxViewData<TMgr, TArg> view;

        public override bool IsIncremental { get; }

        public ChoicesMenuScreen(string message, bool isIncremental = true, string speechBoxSubviewName = null, string choicesSubviewName = null)
        {
            getMessage = delegate { return message; };
            IsIncremental = isIncremental;

            view = new()
            {
            };
            if (speechBoxSubviewName != null) { view.SpeechBoxSubviewName = speechBoxSubviewName; }
            if (choicesSubviewName != null) { view.ChoicesSubviewName = choicesSubviewName; }
        }

        public ChoicesMenuScreen(ItemNameSelector<TMgr, TArg> getMessage, bool isIncremental = true, string speechBoxSubviewName = null, string choicesSubviewName = null)
        {
            this.getMessage = getMessage;
            IsIncremental = isIncremental;

            view = new()
            {
            };
            if (speechBoxSubviewName != null) { view.SpeechBoxSubviewName = speechBoxSubviewName; }
            if (choicesSubviewName != null) { view.ChoicesSubviewName = choicesSubviewName; }
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
