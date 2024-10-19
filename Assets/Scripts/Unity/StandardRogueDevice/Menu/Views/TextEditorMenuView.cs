using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;
using Roguegard.Rgpacks.MoonSharp;
using TMPro;

namespace RoguegardUnity
{
    public class TextEditorMenuView : ElementsView//, ITextElementsView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private TMP_InputField _inputField = null;

        public CanvasGroup CanvasGroup => _canvasGroup;

        public string Text => _inputField.text;

        public void Initialize()
        {
            _inputField.onEndEdit.AddListener(x => SetArg(Root, Self, User, new(tool: Arg.Tool, other: x)));
        }

        public void SetText(string text)
        {
            _inputField.text = text;
        }

        public void Show(bool show)
        {
            if (show)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
            else
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
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
