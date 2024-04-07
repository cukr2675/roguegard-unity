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
    public class LongDownMenu : IModelsMenu
    {
        private readonly IModelsMenuChoice[] choices;
        private readonly IModelsMenu commandMenu;

        public LongDownMenu(ObjsMenu objsMenu, ObjCommandMenu objCommandMenu)
        {
            choices = new IModelsMenuChoice[]
            {
                objCommandMenu.Summary,
                objCommandMenu.Details,
                objsMenu.Close
            };
            commandMenu = objCommandMenu;
        }

        void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (arg.TargetObj != null && arg.TargetObj.HasCollider && (self.Position - arg.TargetObj.Position).sqrMagnitude <= 2 &&
                RoguegardSettings.ObjCommandTable.Categories.Contains(arg.TargetObj.Main.InfoSet.Category))
            {
                // 長押ししたアイテムと隣接していた場合、アイテム向けのメニューを表示する
                var openArg = new RogueMethodArgument(tool: arg.TargetObj);
                root.OpenMenuAsDialog(commandMenu, self, null, openArg);
                return;
            }

            root.Get(DeviceKw.MenuCommand).OpenView(ChoiceListPresenter.Instance, choices, root, self, user, arg);
        }
    }
}
