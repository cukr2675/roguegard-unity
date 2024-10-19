using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// 長押しメニュー
    /// </summary>
    public class LongDownMenu : RogueMenuScreen
    {
        private readonly IListMenuSelectOption[] selectOptions;
        private readonly RogueMenuScreen commandMenu;

        private readonly MainMenuViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
        };

        public LongDownMenu(ObjsMenu objsMenu, ObjCommandMenu objCommandMenu)
        {
            selectOptions = new IListMenuSelectOption[]
            {
                objCommandMenu.Summary,
                objCommandMenu.Details,
                objsMenu.Close
            };
            commandMenu = objCommandMenu;
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            if (arg.Arg.TargetObj != null && arg.Arg.TargetObj.HasCollider && (arg.Self.Position - arg.Arg.TargetObj.Position).sqrMagnitude <= 2 &&
                RoguegardSettings.ObjCommandTable.Categories.Contains(arg.Arg.TargetObj.Main.InfoSet.Category))
            {
                // 長押ししたアイテムと隣接していた場合、アイテム向けのメニューを表示する
                commandMenu.OpenScreen(manager, new MenuArg(arg.Self, null, new(tool: arg.Arg.TargetObj)).ReadOnly);
                return;
            }

            view.Show(manager, arg)
                ?.AppendRange(selectOptions)
                .Build();
        }
    }
}
