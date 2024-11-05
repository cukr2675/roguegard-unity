using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;
using Roguegard.Rgpacks.MoonSharp;
using TMPro;

namespace RoguegardUnity
{
    public class TextEditorMenuView : ElementsSubView//, ITextElementsView
    {
        [SerializeField] private TMP_InputField _inputField = null;

        public string Text => _inputField.text;

        public void Initialize()
        {
            //_inputField.onEndEdit.AddListener(x => SetArg(Manager, new MenuArg(arg: new(tool: ((ReadOnlyMenuArg)Arg).Arg.Tool, other: x)).ReadOnly));
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider)
        {
            throw new System.NotImplementedException();
        }

        public void SetText(string text)
        {
            _inputField.text = text;
        }

        //public override void OpenView<T>(
        //    IElementPresenter presenter, Spanning<T> list, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //{
        //    SetArg(manager, self, user, arg);
        //    _inputField.text = (string)arg.Other;
        //    MenuController.Show(_canvasGroup, true);
        //}

        //public override float GetPosition()
        //{
        //    throw new System.NotImplementedException();
        //}

        //public override void SetPosition(float position)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
