using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.Device
{
    public class DialogModelsMenuChoice : BaseModelsMenuChoice, IModelsMenu
    {
        public override string Name { get; }

        private readonly string message;
        private readonly IModelsMenuChoice[] choices;

        private DialogModelsMenuChoice(string name, string message, IEnumerable<IModelsMenuChoice> choices)
        {
            Name = name;
            this.message = message;
            this.choices = choices?.ToArray() ?? new IModelsMenuChoice[0];
        }

        private DialogModelsMenuChoice(string name, string message, params IModelsMenuChoice[] choices)
            : this(name, message, (IEnumerable<IModelsMenuChoice>)choices)
        {
        }

        public DialogModelsMenuChoice(params (string, ModelsMenuAction)[] choices)
        {
            this.choices = choices?.Select(x => new ActionModelsMenuChoice(x.Item1, x.Item2)).ToArray() ?? new ActionModelsMenuChoice[0];
        }

        public DialogModelsMenuChoice(string name, string message, params (string, ModelsMenuAction)[] choices)
        {
            Name = name;
            this.message = message;
            this.choices = choices?.Select(x => new ActionModelsMenuChoice(x.Item1, x.Item2)).ToArray() ?? new ActionModelsMenuChoice[0];
        }

        public DialogModelsMenuChoice AppendExit()
        {
            var choice = new DialogModelsMenuChoice(Name, message, choices.Append(ExitModelsMenuChoice.Instance));
            return choice;
        }

        public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            root.OpenMenuAsDialog(this, self, user, arg);
        }

        void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (message != null)
            {
                root.AddInt(DeviceKw.StartTalk, 0);
                root.AddObject(DeviceKw.AppendText, message);
                root.AddInt(DeviceKw.WaitEndOfTalk, 0);
            }

            root.Get(DeviceKw.MenuTalkChoices).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, self, user, arg);
        }

        public static DialogModelsMenuChoice CreateExit(ModelsMenuAction saveAction, ModelsMenuAction notSaveAction = null)
        {
            var choice = CreateExit(":Exit", ":ExitMsg", ":Overwrite", saveAction, ":DontSave", notSaveAction);
            return choice;
        }

        public static DialogModelsMenuChoice CreateExit(
            string name, string message, string saveName, ModelsMenuAction saveAction, string notSaveName, ModelsMenuAction notSaveAction)
        {
            var choice = new DialogModelsMenuChoice(
                name, message,
                new ActionModelsMenuChoice(saveName, saveAction),
                new DialogModelsMenuChoice(
                    notSaveName, ":SecondExitMsg",
                    new ActionModelsMenuChoice(notSaveName, notSaveAction ?? NotSave),
                    ExitModelsMenuChoice.Instance),
                ExitModelsMenuChoice.Instance);
            return choice;
        }

        private static void NotSave(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            // ‰½‚à‚¹‚¸•Â‚¶‚é
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
            root.Back();
            root.Back();
        }
    }
}
