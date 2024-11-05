using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
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

        private class Menu : RogueMenuScreen
        {
            private static readonly object[] backAnchor = new object[]
            {
                SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("é¿çs", Execute),
                SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("ï¬Ç∂ÇÈ", Back),
            };

            private IElementsSubViewStateProvider stateProvider;

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                var memo = arg.Arg.Tool;
                var text = NotepadInfo.GetText(memo);

                manager.OpenTextEditor(text);
                manager.StandardSubViewTable.BackAnchor.Show(backAnchor, SelectOptionHandler.Instance, manager, arg, ref stateProvider);
            }

            private static void Back(RogueMenuManager manager, ReadOnlyMenuArg arg)
            {
                NotepadInfo.SetTo(arg.Arg.Tool, manager.TextEditorValue);
                manager.Done();
            }

            private static void Execute(RogueMenuManager manager, ReadOnlyMenuArg arg)
            {
                //var scroll = manager.GetView(DeviceKw.MenuTextEditor);
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
