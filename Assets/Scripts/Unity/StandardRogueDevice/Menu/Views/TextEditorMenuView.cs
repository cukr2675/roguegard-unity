using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;
using Roguegard.Scripting.MoonSharp;
using TMPro;

namespace RoguegardUnity
{
    public class TextEditorMenuView : ModelsMenuView, IScrollModelsMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private TMP_InputField _inputField = null;
        [SerializeField] private ModelsMenuViewItemButton _executeButton = null;
        [SerializeField] private ModelsMenuViewItemButton _exitButton = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        public void Initialize()
        {
            _executeButton.Initialize(this);
            _executeButton.SetItem(ChoicesModelsMenuItemController.Instance, new Execute() { parent = this });
            _exitButton.Initialize(this);
            _inputField.onEndEdit.AddListener(x => SetArg(Root, Self, User, new(tool: Arg.Tool, other: x)));
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models,
            IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(root, self, user, arg);
            _inputField.text = (string)arg.Other;
            MenuController.Show(_exitButton.CanvasGroup, false);
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

        public void ShowExitButton(IModelsMenuChoice choice)
        {
            _exitButton.SetItem(ChoicesModelsMenuItemController.Instance, choice);
            MenuController.Show(_exitButton.CanvasGroup, true);
        }

        private class Execute : IModelsMenuChoice
        {
            public TextEditorMenuView parent;

            private static readonly MoonSharpRogueScript script = new MoonSharpRogueScript();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "実行";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (parent._inputField.text.StartsWith("#!lua"))
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    NotepadInfo.SetTo(arg.Tool, parent._inputField.text);
                    var code = NotepadInfo.GetQuote(arg.Tool);
                    script.Call(code, self);
                }
                else
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                }
            }
        }
    }
}
