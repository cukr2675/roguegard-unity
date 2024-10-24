//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using System.Linq;
//using ListingMF;

//namespace Roguegard.Device
//{
//    public class DialogListMenuSelectOption : BaseListMenuSelectOption
//    {
//        private readonly string message;
//        private readonly IListMenuSelectOption[] selectOptions;
//        public readonly MenuScreen menuScreen;

//        private DialogListMenuSelectOption(string name, string message, IEnumerable<IListMenuSelectOption> selectOptions)
//        {
//            Name = name;
//            menuScreen = new MenuScreen()
//            {
//                message = message ?? "    ",
//                selectOptions = selectOptions?.ToArray() ?? new IListMenuSelectOption[0],
//            };

//            HandleClick = (manager, arg) =>
//            {
//                manager.PushMenuScreen(menuScreen, arg);
//            };
//        }

//        private DialogListMenuSelectOption(string name, string message, params IListMenuSelectOption[] selectOptions)
//            : this(name, message, (IEnumerable<IListMenuSelectOption>)selectOptions)
//        {
//        }

//        public DialogListMenuSelectOption(params (string, HandleClickAction)[] selectOptions)
//        {
//            this.selectOptions = selectOptions?.Select(x => new ActionListMenuSelectOption(x.Item1, x.Item2)).ToArray() ?? new ActionListMenuSelectOption[0];
//        }

//        public DialogListMenuSelectOption(string name, string message, params (string, HandleClickAction)[] selectOptions)
//        {
//            Name = name;
//            this.message = message;
//            this.selectOptions = selectOptions?.Select(x => new ActionListMenuSelectOption(x.Item1, x.Item2)).ToArray() ?? new ActionListMenuSelectOption[0];
//        }

//        public DialogListMenuSelectOption AppendExit()
//        {
//            var selectOption = new DialogListMenuSelectOption(Name, message, selectOptions.Append(ExitListMenuSelectOption.Instance));
//            return selectOption;
//        }

//        public static DialogListMenuSelectOption CreateExit(HandleClickAction saveAction, HandleClickAction notSaveAction = null)
//        {
//            var selectOption = CreateExit(":Exit", ":ExitMsg", ":Overwrite", saveAction, ":DontSave", notSaveAction);
//            return selectOption;
//        }

//        public static DialogListMenuSelectOption CreateExit(
//            string name, string message, string saveName, HandleClickAction saveAction, string notSaveName, HandleClickAction notSaveAction)
//        {
//            var selectOption = new DialogListMenuSelectOption(
//                name, message,
//                new ActionListMenuSelectOption(saveName, saveAction),
//                new DialogListMenuSelectOption(
//                    notSaveName, ":SecondExitMsg",
//                    new ActionListMenuSelectOption(notSaveName, notSaveAction ?? NotSave),
//                    ExitListMenuSelectOption.Instance),
//                ExitListMenuSelectOption.Instance);
//            return selectOption;
//        }

//        private static void NotSave(IListMenuManager manager, ReadOnlyMenuArg arg)
//        {
//            // ‰½‚à‚¹‚¸•Â‚¶‚é
//            manager.HandleClickBack();
//            manager.HandleClickBack();
//        }

//        public class MenuScreen : RogueMenuScreen
//        {
//            public string message;
//            public IListMenuSelectOption[] selectOptions;

//            private readonly SpeechBoxViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
//            {
//            };

//            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
//            {
//                view.Show(message, manager, arg)
//                    ?.AppendRange(selectOptions)
//                    .Build();
//            }
//        }
//    }
//}
