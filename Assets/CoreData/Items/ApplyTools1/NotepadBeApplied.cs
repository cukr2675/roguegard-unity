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

        private class Menu : IListMenu
        {
            private static readonly object[] exit = new object[]
            {
                new ActionListMenuSelectOption("é¿çs", Execute),
                new ActionListMenuSelectOption("ï¬Ç∂ÇÈ", Exit),
            };

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var memo = arg.Tool;
                var text = NotepadInfo.GetText(memo);
                var scroll = manager.GetView(DeviceKw.MenuTextEditor);
                scroll.OpenView(SelectOptionPresenter.Instance, Spanning<object>.Empty, manager, self, null, new(tool: memo, other: text));

                var leftAnchor = manager.GetView(DeviceKw.MenuLeftAnchor);
                leftAnchor.OpenView(SelectOptionPresenter.Instance, exit, manager, null, null, new(tool: memo));
            }

            private static void Exit(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var scroll = (ITextElementsView)manager.GetView(DeviceKw.MenuTextEditor);
                NotepadInfo.SetTo(arg.Tool, scroll.Text);
                manager.Done();
            }

            private static void Execute(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var scroll = manager.GetView(DeviceKw.MenuTextEditor);
                //if (parent._inputField.text.StartsWith("#!lua"))
                //{
                //    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                //    NotepadInfo.SetTo(arg.Tool, parent._inputField.text);
                //    var code = NotepadInfo.GetQuote(arg.Tool);
                //    script.Call(code, self);
                //}
                //else
                //{
                //    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                //}
            }
        }
    }
}
