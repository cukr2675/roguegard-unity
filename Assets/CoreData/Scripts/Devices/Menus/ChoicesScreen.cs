using Lysionium;

namespace Roguegard.Device
{
    /// <summary>
    /// メッセージと選択肢のメニュー画面
    /// </summary>
    public class ChoicesScreen : RogueListuiScreen, ISelectOptionsBuilder<MMgr, MArg, ChoicesScreen>
    {
        private readonly ChoicesScreen<MMgr, MArg> screen;

        public override bool IsIncremental => screen.IsIncremental;

        public ChoicesScreen(string message)
        {
            screen = new ChoicesScreen<MMgr, MArg>(message);
        }

        public ChoicesScreen(System.Func<MMgr, MArg, string> getMessage)
        {
            screen = new ChoicesScreen<MMgr, MArg>(getMessage);
        }

        /// <summary>
        /// 「保存して戻りますか？」のダイアログ画面を生成する
        /// </summary>
        public static ChoicesScreen SaveBackDialog(
            ClickItemHandler<MMgr, MArg> saveAction,
            ClickItemHandler<MMgr, MArg> notSaveAction = null)
        {
            var selectOption = SaveBackDialog(":SaveBackDialogMsg", ":Overwrite", saveAction, ":DontSave", notSaveAction);
            return selectOption;
        }

        /// <summary>
        /// 「保存して戻りますか？」のダイアログ画面を生成する
        /// </summary>
        public static ChoicesScreen SaveBackDialog(
            string message,
            string saveName, ClickItemHandler<MMgr, MArg> saveAction,
            string notSaveName, ClickItemHandler<MMgr, MArg> notSaveAction)
        {
            var selectOption = new ChoicesScreen(message)

                // 保存
                .Option(saveName, saveAction)

                // 保存しない場合は再度聞く
                .Option(notSaveName, new ChoicesScreen(":SaveBackDialogMsg::Second").Option(notSaveName, notSaveAction ?? NotSave).Option(":Cancel", Cancel))

                .Option(":Cancel", (manager, arg) => manager.PopScreen());

            return selectOption;
        }

        private static void NotSave(MMgr manager, MArg arg)
        {
            // 何もせず閉じる
            manager.PopScreen(3);
        }

        private static void Cancel(MMgr manager, MArg arg)
        {
            // 何もせず閉じる
            manager.PopScreen(2);
        }

        public ChoicesScreen Option(ISelectOption<MMgr, MArg> option)
        {
            screen.Option(option);
            return this;
        }

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            screen.OpenScreen(manager, arg);
        }

        public override void CloseScreenView(MMgr manager, bool back)
        {
            screen.CloseScreenView(manager, back);
        }

        ChoicesScreen ISelectOptionsBuilder<MMgr, MArg, ChoicesScreen>.Option() => this;
    }
}
