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

        public static ChoicesMenuScreen CreateExit(
            HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> saveAction,
            HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> notSaveAction = null)
        {
            var selectOption = CreateExit(":ExitMsg", ":Overwrite", saveAction, ":DontSave", notSaveAction);
            return selectOption;
        }

        public static ChoicesMenuScreen CreateExit(
            string message,
            string saveName, HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> saveAction,
            string notSaveName, HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> notSaveAction)
        {
            var selectOption = new ChoicesMenuScreen(message)

                // ï€ë∂
                .Option(saveName, saveAction)

                // ï€ë∂ÇµÇ»Ç¢èÍçáÇÕçƒìxï∑Ç≠
                .Option(notSaveName, new ChoicesMenuScreen(":SecondExitMsg").Option(notSaveName, notSaveAction ?? NotSave).Exit())

                .Exit();

            return selectOption;
        }

        private static void NotSave(RogueMenuManager manager, ReadOnlyMenuArg arg)
        {
            // âΩÇ‡ÇπÇ∏ï¬Ç∂ÇÈ
            manager.HandleClickBack();
            manager.HandleClickBack();
            manager.HandleClickBack();
        }

        public ChoicesMenuScreen Option(string name, HandleClickElement<RogueMenuManager, ReadOnlyMenuArg> handleClick)
        {
            screen.Option(name, handleClick);
            return this;
        }

        public ChoicesMenuScreen Exit()
        {
            screen.Exit();
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
