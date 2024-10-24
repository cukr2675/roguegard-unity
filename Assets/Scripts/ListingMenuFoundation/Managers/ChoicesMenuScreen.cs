using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class ChoicesMenuScreen<TMgr, TArg> : MenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly GetElementName<TMgr, TArg> getMessage;
        private readonly List<IListMenuSelectOption> selectOptions = new();
        private readonly SpeechBoxViewTemplate<TMgr, TArg> view;

        public override bool IsIncremental => true;

        public ChoicesMenuScreen(string message)
        {
            getMessage = delegate { return message; };

            view = new()
            {
            };
        }

        public ChoicesMenuScreen(GetElementName<TMgr, TArg> getMessage)
        {
            this.getMessage = getMessage;

            view = new()
            {
            };
        }

        public ChoicesMenuScreen<TMgr, TArg> Option(string name, HandleClickElement<TMgr, TArg> handleClick)
        {
            selectOptions.Add(ListMenuSelectOption.Create(name, handleClick));
            return this;
        }

        public ChoicesMenuScreen<TMgr, TArg> Exit()
        {
            selectOptions.Add(ExitListMenuSelectOption.Instance);
            return this;
        }

        public override void OpenScreen(in TMgr manager, in TArg arg)
        {
            var message = getMessage(manager, arg);

            view.Show(message, manager, arg)
                ?
                .AppendRange(selectOptions)

                .Build();
        }

        public override void CloseScreen(TMgr manager, bool back)
        {
            view.HideSubViews(manager, back);
        }
    }
}
