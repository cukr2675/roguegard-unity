using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public class ChoicesMenuScreen : RogueMenuScreen
    {
        private readonly ChoicesMenuScreen<RogueMenuManager, ReadOnlyMenuArg> screen;

        public override bool IsIncremental => screen.IsIncremental;

        public ChoicesMenuScreen(string message)
        {
            screen = new ChoicesMenuScreen<RogueMenuManager, ReadOnlyMenuArg>(message);
        }

        public ChoicesMenuScreen(GetElementName<RogueMenuManager, ReadOnlyMenuArg> getMessage)
        {
            screen = new ChoicesMenuScreen<RogueMenuManager, ReadOnlyMenuArg>(getMessage);
        }

        public static ChoicesMenuScreen SaveBackDialog(
            HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> saveAction,
            HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> notSaveAction = null)
        {
            var selectOption = SaveBackDialog(":SaveBackDialogMsg", ":Overwrite", saveAction, ":DontSave", notSaveAction);
            return selectOption;
        }

        public static ChoicesMenuScreen SaveBackDialog(
            string message,
            string saveName, HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> saveAction,
            string notSaveName, HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> notSaveAction)
        {
            var selectOption = new ChoicesMenuScreen(message)

                // •Û‘¶
                .Option(saveName, saveAction)

                // •Û‘¶‚µ‚È‚¢ê‡‚ÍÄ“x•·‚­
                .Option(notSaveName, new ChoicesMenuScreen(":SaveBackDialogMsg::Second").Option(notSaveName, notSaveAction ?? NotSave).Option(":Cancel", Cancel))

                .Option(":Cancel", (manager, arg) => manager.Back());

            return selectOption;
        }

        private static void NotSave(RogueMenuManager manager, ReadOnlyMenuArg arg)
        {
            // ‰½‚à‚¹‚¸•Â‚¶‚é
            manager.Back(3);
        }

        private static void Cancel(RogueMenuManager manager, ReadOnlyMenuArg arg)
        {
            // ‰½‚à‚¹‚¸•Â‚¶‚é
            manager.Back(2);
        }

        public ChoicesMenuScreen Option(string name, HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> handleClick)
        {
            screen.Option(name, handleClick);
            return this;
        }

        public ChoicesMenuScreen Back()
        {
            screen.Back();
            return this;
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            screen.OpenScreen(manager, arg);
        }

        public override void CloseScreen(RogueMenuManager manager, bool back)
        {
            screen.CloseScreen(manager, back);
        }
    }
}
