using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    public class NotepadBeApplied : BaseApplyRogueMethod
    {
        private static readonly Menu menu = new Menu();

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            RogueDevice.Primary.AddMenu(menu, user, null, new(tool: self));
            return false;
        }

        private class Menu : IModelsMenu
        {
            private static readonly object[] exit = new object[]
            {
                new ActionModelsMenuChoice("é¿çs", Execute),
                new ActionModelsMenuChoice("ï¬Ç∂ÇÈ", Exit),
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var memo = arg.Tool;
                var text = NotepadInfo.GetText(memo);
                var scroll = root.Get(DeviceKw.MenuTextEditor);
                scroll.OpenView(ChoiceListPresenter.Instance, Spanning<object>.Empty, root, self, null, new(tool: memo, other: text));

                var leftAnchor = root.Get(DeviceKw.MenuLeftAnchor);
                leftAnchor.OpenView(ChoiceListPresenter.Instance, exit, root, null, null, new(tool: memo));
            }

            private static void Exit(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var scroll = (ITextMenuView)root.Get(DeviceKw.MenuTextEditor);
                NotepadInfo.SetTo(arg.Tool, scroll.Text);
                root.Done();
            }

            private static void Execute(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var scroll = root.Get(DeviceKw.MenuTextEditor);
                //if (parent._inputField.text.StartsWith("#!lua"))
                //{
                //    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                //    NotepadInfo.SetTo(arg.Tool, parent._inputField.text);
                //    var code = NotepadInfo.GetQuote(arg.Tool);
                //    script.Call(code, self);
                //}
                //else
                //{
                //    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                //}
            }
        }
    }
}
