using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// 長押しメニュー
    /// </summary>
    public class LongDownMenu : IListMenu
    {
        private readonly IListMenuSelectOption[] selectOptions;
        private readonly IListMenu commandMenu;

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

        void IListMenu.OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (arg.TargetObj != null && arg.TargetObj.HasCollider && (self.Position - arg.TargetObj.Position).sqrMagnitude <= 2 &&
                RoguegardSettings.ObjCommandTable.Categories.Contains(arg.TargetObj.Main.InfoSet.Category))
            {
                // 長押ししたアイテムと隣接していた場合、アイテム向けのメニューを表示する
                var openArg = new RogueMethodArgument(tool: arg.TargetObj);
                manager.OpenMenuAsDialog(commandMenu, self, null, openArg);
                return;
            }

            manager.GetView(DeviceKw.MenuCommand).OpenView(SelectOptionPresenter.Instance, selectOptions, manager, self, user, arg);
        }
    }
}
