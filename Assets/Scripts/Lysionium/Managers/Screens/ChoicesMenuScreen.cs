using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public class ChoicesMenuScreen<TMgr, TArg> : MenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly GetElementName<TMgr, TArg> getMessage;
        private readonly List<ISelectOption> selectOptions = new();
        private readonly SpeechBoxViewTemplate<TMgr, TArg> view;

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

        public ChoicesMenuScreen(GetElementName<TMgr, TArg> getMessage, bool isIncremental = true, string speechBoxSubviewName = null, string choicesSubviewName = null)
        {
            this.getMessage = getMessage;
            IsIncremental = isIncremental;

            view = new()
            {
            };
            if (speechBoxSubviewName != null) { view.SpeechBoxSubviewName = speechBoxSubviewName; }
            if (choicesSubviewName != null) { view.ChoicesSubviewName = choicesSubviewName; }
        }

        public ChoicesMenuScreen<TMgr, TArg> Option(string name, HandleClickElement<TMgr, TArg> onClick)
        {
            selectOptions.Add(SelectOption.Create(name, onClick));
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

            view.ShowTemplate(message, manager, arg)
                ?
                .TailRange(selectOptions)

                .Build();
        }

        public override void CloseScreenView(TMgr manager, bool back)
        {
            if (IsIncremental) { view.HideTemplate(manager, back); }
            else { base.CloseScreenView(manager, back); }
        }
    }
}
