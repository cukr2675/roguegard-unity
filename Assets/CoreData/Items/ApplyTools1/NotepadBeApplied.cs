using Lysionium;
using Roguegard.Device;

namespace Roguegard
{
    public class NotepadBeApplied : BaseApplyRogueMethod
    {
        private static readonly NotepadScreen notepadScreen = new();

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            RogueDevice.Primary.AddScreen(notepadScreen, user, null, new(tool: self));
            return false;
        }

        private class NotepadScreen : RogueListuiScreen
        {
            private ISubviewStateProvider stateProvider;

            public NotepadScreen()
            {
                var backAnchor = new SelectOptionList<MMgr>(
                    _ => _
                    .Option("実行", Execute)
                    .Option("閉じる", Back));

                OnOpenScreen += (manager) =>
                {
                    var memo = Arg.Arg.Tool;
                    var text = NotepadInfo.GetText(memo);

                    manager.TextEditor.Text = text;
                    manager.TextEditor.Show();
                    manager.BackAnchor.Show(backAnchor, manager, ref stateProvider);
                };
            }

            private void Back(MMgr manager)
            {
                NotepadInfo.SetTo(Arg.Arg.Tool, manager.TextEditor.Text);
                manager.Done();
            }

            private void Execute(MMgr manager)
            {
                //var scroll = manager.GetView(DeviceKw.MenuTextEditor);
                //if (parent._inputField.text.StartsWith("#!lua"))
                //{
                //    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                //    NotepadInfo.SetTo(Arg.Tool, parent._inputField.text);
                //    var code = NotepadInfo.GetQuote(Arg.Tool);
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
