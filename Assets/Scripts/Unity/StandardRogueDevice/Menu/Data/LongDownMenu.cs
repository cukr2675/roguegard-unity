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
        private readonly ISelectOption[] selectOptions;
        private readonly RogueMenuScreen commandMenu;

        private readonly MainMenuViewTemplate<MMgr, MArg> view = new()
        {
        };

        public LongDownMenu(ObjsMenu objsMenu, ObjCommandMenu objCommandMenu)
        {
            selectOptions = new ISelectOption[]
            {
                objCommandMenu.Summary,
                objCommandMenu.Details,
                objsMenu.Close
            };
            commandMenu = objCommandMenu;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            if (arg.Arg.TargetObj != null && arg.Arg.TargetObj.HasCollider && (arg.Self.Position - arg.Arg.TargetObj.Position).sqrMagnitude <= 2 &&
                RoguegardSettings.ObjCommandTable.Categories.Contains(arg.Arg.TargetObj.Main.InfoSet.Category))
            {
                // 長押ししたアイテムと隣接していた場合、アイテム向けのメニューを表示する
                commandMenu.OpenScreen(manager, new MArg.Builder(arg.Self, null, new(tool: arg.Arg.TargetObj)).ReadOnly);
                return;
            }

            view.ShowTemplate(manager, arg)
                ?.AppendRange(selectOptions)
                .Build();
        }
    }
}
