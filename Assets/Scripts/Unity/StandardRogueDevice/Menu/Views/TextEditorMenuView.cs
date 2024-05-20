using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;
using Roguegard.Scripting.MoonSharp;
using TMPro;

namespace RoguegardUnity
{
    public class TextEditorMenuView : ModelsMenuView, ITextMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private TMP_InputField _inputField = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        public string Text => _inputField.text;

        public void Initialize()
        {
            _inputField.onEndEdit.AddListener(x => SetArg(Root, Self, User, new(tool: Arg.Tool, other: x)));
        }

        public override void OpenView<T>(
            IModelListPresenter presenter, Spanning<T> modelList, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(root, self, user, arg);
            _inputField.text = (string)arg.Other;
            MenuController.Show(_canvasGroup, true);
        }

        public override float GetPosition()
        {
            throw new System.NotImplementedException();
        }

        public override void SetPosition(float position)
        {
            throw new System.NotImplementedException();
        }
    }
}
