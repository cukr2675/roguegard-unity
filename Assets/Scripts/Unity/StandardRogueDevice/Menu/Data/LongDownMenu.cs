using Lysionium;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// 長押しメニュー
    /// </summary>
    public class LongDownMenu : RogueMenuScreen
    {
        private readonly ISelectOption<MMgr, MArg>[] selectOptions;
        private readonly RogueMenuScreen commandMenu;

        private readonly MainMenuViewData<MMgr, MArg> view = new()
        {
        };

        public LongDownMenu(ObjsMenu objsMenu, ObjCommandMenu objCommandMenu)
        {
            selectOptions = new ISelectOption<MMgr, MArg>[]
            {
                objCommandMenu.Summary,
                objCommandMenu.Details,
                objsMenu.Close
            };
            commandMenu = objCommandMenu;
        }

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            if (arg.Arg.TargetObj != null && arg.Arg.TargetObj.HasCollider && (arg.Self.Position - arg.Arg.TargetObj.Position).sqrMagnitude <= 2 &&
                RoguegardSettings.ObjCommandTable.Categories.Contains(arg.Arg.TargetObj.Main.InfoSet.Category))
            {
                // 長押ししたアイテムと隣接していた場合、アイテム向けのメニューを表示する
                commandMenu.OpenScreen(manager, new MArg.Builder(arg.Self, null, new(tool: arg.Arg.TargetObj)).ReadOnly);
                return;
            }

            view.Show(manager, arg)
                ?
                .Tail.OptionRange(selectOptions)

                .Build();
        }
    }
}
