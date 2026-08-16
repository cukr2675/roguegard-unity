using Lysionium;

namespace Roguegard.Device
{
    /// <summary>
    /// メッセージと選択肢のメニュー画面
    /// </summary>
    public class ChoicesScreen : RogueListuiScreen, ISelectOptionsBuilder<MMgr, ChoicesScreen>
    {
        private readonly ChoicesScreen<MMgr> screen;

        public ChoicesScreen(string message)
        {
            screen = new ChoicesScreen<MMgr>(message, isIncremental: true);
            OnOpenScreen += screen.OpenScreen;
            OnCloseScreenView += screen.CloseScreenView;
        }

        public ChoicesScreen(System.Func<MMgr, string> getMessage)
        {
            screen = new ChoicesScreen<MMgr>(getMessage, isIncremental: true);
            OnOpenScreen += screen.OpenScreen;
            OnCloseScreenView += screen.CloseScreenView;
        }

        /// <summary>
        /// 「保存して戻りますか？」のダイアログ画面を生成する
        /// </summary>
        public static ChoicesScreen SaveBackDialog(
            System.Action<MMgr> saveAction,
            System.Action<MMgr> notSaveAction = null)
        {
            var selectOption = SaveBackDialog(":SaveBackDialogMsg", ":Overwrite", saveAction, ":DontSave", notSaveAction);
            return selectOption;
        }

        /// <summary>
        /// 「保存して戻りますか？」のダイアログ画面を生成する
        /// </summary>
        public static ChoicesScreen SaveBackDialog(
            string message,
            string saveName, System.Action<MMgr> saveAction,
            string notSaveName, System.Action<MMgr> notSaveAction)
        {
            var selectOption = new ChoicesScreen(message);
            selectOption

                // 保存
                .Option(saveName, saveAction)

                // 保存しない場合は再度聞く
                .Option(
                    notSaveName,
                    new ChoicesScreen(":SaveBackDialogMsg::Second").Option(notSaveName, notSaveAction ?? NotSave).Option(":Cancel", Cancel),
                    () => selectOption.Arg)

                .Option(":Cancel", (manager) => manager.PopScreen());

            return selectOption;
        }

        private static void NotSave(MMgr manager)
        {
            // 何もせず閉じる
            manager.PopScreen(3);
        }

        private static void Cancel(MMgr manager)
        {
            // 何もせず閉じる
            manager.PopScreen(2);
        }

        public ChoicesScreen Option(ISelectOption<MMgr> option)
        {
            screen.Option(option);
            return this;
        }

        ChoicesScreen ISelectOptionsBuilder<MMgr, ChoicesScreen>.Option() => this;
    }
}
