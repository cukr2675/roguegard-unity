using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.Device
{
    public class DialogListMenuSelectOption : BaseListMenuSelectOption, IListMenu
    {
        public override string Name { get; }

        private readonly string message;
        private readonly IListMenuSelectOption[] selectOptions;

        private DialogListMenuSelectOption(string name, string message, IEnumerable<IListMenuSelectOption> selectOptions)
        {
            Name = name;
            this.message = message;
            this.selectOptions = selectOptions?.ToArray() ?? new IListMenuSelectOption[0];
        }

        private DialogListMenuSelectOption(string name, string message, params IListMenuSelectOption[] selectOptions)
            : this(name, message, (IEnumerable<IListMenuSelectOption>)selectOptions)
        {
        }

        public DialogListMenuSelectOption(params (string, ListMenuAction)[] selectOptions)
        {
            this.selectOptions = selectOptions?.Select(x => new ActionListMenuSelectOption(x.Item1, x.Item2)).ToArray() ?? new ActionListMenuSelectOption[0];
        }

        public DialogListMenuSelectOption(string name, string message, params (string, ListMenuAction)[] selectOptions)
        {
            Name = name;
            this.message = message;
            this.selectOptions = selectOptions?.Select(x => new ActionListMenuSelectOption(x.Item1, x.Item2)).ToArray() ?? new ActionListMenuSelectOption[0];
        }

        public DialogListMenuSelectOption AppendExit()
        {
            var selectOption = new DialogListMenuSelectOption(Name, message, selectOptions.Append(ExitListMenuSelectOption.Instance));
            return selectOption;
        }

        public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            manager.OpenMenuAsDialog(this, self, user, arg);
        }

        void IListMenu.OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (message != null)
            {
                manager.AddInt(DeviceKw.StartTalk, 0);
                manager.AddObject(DeviceKw.AppendText, message);
                manager.AddInt(DeviceKw.WaitEndOfTalk, 0);
            }

            manager.GetView(DeviceKw.MenuTalkSelect).OpenView(SelectOptionPresenter.Instance, selectOptions, manager, self, user, arg);
        }

        public static DialogListMenuSelectOption CreateExit(ListMenuAction saveAction, ListMenuAction notSaveAction = null)
        {
            var selectOption = CreateExit(":Exit", ":ExitMsg", ":Overwrite", saveAction, ":DontSave", notSaveAction);
            return selectOption;
        }

        public static DialogListMenuSelectOption CreateExit(
            string name, string message, string saveName, ListMenuAction saveAction, string notSaveName, ListMenuAction notSaveAction)
        {
            var selectOption = new DialogListMenuSelectOption(
                name, message,
                new ActionListMenuSelectOption(saveName, saveAction),
                new DialogListMenuSelectOption(
                    notSaveName, ":SecondExitMsg",
                    new ActionListMenuSelectOption(notSaveName, notSaveAction ?? NotSave),
                    ExitListMenuSelectOption.Instance),
                ExitListMenuSelectOption.Instance);
            return selectOption;
        }

        private static void NotSave(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            // ‰½‚à‚¹‚¸•Â‚¶‚é
            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
            manager.Back();
            manager.Back();
        }
    }
}
