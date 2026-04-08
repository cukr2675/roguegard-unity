using Lysionium;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// 長押しメニュー
    /// </summary>
    public class LongDownMenu : RogueListuiScreen
    {
        private readonly ISelectOption<MMgr, MArg>[] selectOptions;
        private readonly RogueListuiScreen commandMenu;

        private readonly ScrollMenuViewData<ISelectOption<MMgr, MArg>, MMgr> view = new()
        {
        };

        public LongDownMenu(ObjsMenu objsMenu, ObjCommandMenuScreen objCommandMenuScreen)
        {
            selectOptions = new ISelectOption<MMgr, MArg>[]
            {
                objCommandMenuScreen.Summary,
                objCommandMenuScreen.Details,
                objsMenu.Close
            };
            commandMenu = objCommandMenuScreen;
        }

        public LongDownMenu()
        {
            OnOpenScreen += (manager) =>
            {
                if (Arg.Arg.TargetObj != null && Arg.Arg.TargetObj.HasCollider && (Arg.Self.Position - Arg.Arg.TargetObj.Position).sqrMagnitude <= 2 &&
                    RoguegardSettings.ObjCommandTable.Categories.Contains(Arg.Arg.TargetObj.Main.InfoSet.Category))
                {
                    // 長押ししたアイテムと隣接していた場合、アイテム向けのメニューを表示する
                    ((IListuiScreen<MMgrBase, MArg>)commandMenu).OpenScreen(
                        manager, new MArg.Builder(Arg.Self, null, new(tool: Arg.Arg.TargetObj)).ReadOnly);
                    return;
                }

                view.Show(selectOptions, manager)
                ?
                .NameFrom((o, m) => o.GetName(m, Arg))
                .OnClick((o, m) => o.Click(m, Arg))
                .StyleFrom((o, m) => o.GetStyle(m, Arg))

                .Build();
            };
        }
    }
}
