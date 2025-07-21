using Lysionium;

namespace Roguegard.Device
{
    /// <summary>
    /// メッセージと選択肢のメニュー画面
    /// </summary>
    public class ChoicesMenuScreen : RogueMenuScreen
    {
        private readonly ChoicesMenuScreen<MMgr, MArg> screen;

        public override bool IsIncremental => screen.IsIncremental;

        public ChoicesMenuScreen(string message)
        {
            screen = new ChoicesMenuScreen<MMgr, MArg>(message);
        }

        public ChoicesMenuScreen(GetElementName<MMgr, MArg> getMessage)
        {
            screen = new ChoicesMenuScreen<MMgr, MArg>(getMessage);
        }

        /// <summary>
        /// 「保存して戻りますか？」のダイアログ画面を生成する
        /// </summary>
        public static ChoicesMenuScreen SaveBackDialog(
            HandleClickElement<MMgr, MArg> saveAction,
            HandleClickElement<MMgr, MArg> notSaveAction = null)
        {
            var selectOption = SaveBackDialog(":SaveBackDialogMsg", ":Overwrite", saveAction, ":DontSave", notSaveAction);
            return selectOption;
        }

        /// <summary>
        /// 「保存して戻りますか？」のダイアログ画面を生成する
        /// </summary>
        public static ChoicesMenuScreen SaveBackDialog(
            string message,
            string saveName, HandleClickElement<MMgr, MArg> saveAction,
            string notSaveName, HandleClickElement<MMgr, MArg> notSaveAction)
        {
            var selectOption = new ChoicesMenuScreen(message)

                // 保存
                .Option(saveName, saveAction)

                // 保存しない場合は再度聞く
                .Option(notSaveName, new ChoicesMenuScreen(":SaveBackDialogMsg::Second").Option(notSaveName, notSaveAction ?? NotSave).Option(":Cancel", Cancel))

                .Option(":Cancel", (manager, arg) => manager.PopMenuScreen());

            return selectOption;
        }

        private static void NotSave(MMgr manager, MArg arg)
        {
            // 何もせず閉じる
            manager.PopMenuScreen(3);
        }

        private static void Cancel(MMgr manager, MArg arg)
        {
            // 何もせず閉じる
            manager.PopMenuScreen(2);
        }

        public ChoicesMenuScreen Option(string name, HandleClickElement<MMgr, MArg> onClick)
        {
            screen.Option(name, onClick);
            return this;
        }

        public ChoicesMenuScreen Back()
        {
            screen.Back();
            return this;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            screen.OpenScreen(manager, arg);
        }

        public override void CloseScreenView(MMgr manager, bool back)
        {
            screen.CloseScreenView(manager, back);
        }
    }
}
