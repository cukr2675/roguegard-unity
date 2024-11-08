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
                SelectOption.Create<MMgr, MArg>("é¿çs", Execute),
                SelectOption.Create<MMgr, MArg>("ï¬Ç∂ÇÈ", Back),
            };

            private IElementsSubViewStateProvider stateProvider;

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var memo = arg.Arg.Tool;
                var text = NotepadInfo.GetText(memo);

                var textEditor = RoguegardSubViews.GetTextEditor(manager);
                textEditor.Text = text;
                textEditor.Show();
                manager.StandardSubViewTable.BackAnchor.Show(backAnchor, SelectOptionHandler.Instance, manager, arg, ref stateProvider);
            }

            private static void Back(MMgr manager, MArg arg)
            {
                var textEditor = RoguegardSubViews.GetTextEditor(manager);
                NotepadInfo.SetTo(arg.Arg.Tool, textEditor.Text);
                manager.Done();
            }

            private static void Execute(MMgr manager, MArg arg)
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
