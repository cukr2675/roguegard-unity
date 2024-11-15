using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class ChestDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly StandardRogueDeviceComponentManager componentManager;
        private readonly RogueMenuScreen putIntoChestMenu;
        private readonly RogueMenuScreen takeOutFromChestMenu;

        public ChestDeviceEventHandler(StandardRogueDeviceComponentManager componentManager)
        {
            this.componentManager = componentManager;

            var putInCommandMenu = new PutIntoChestCommandMenu();
            var takeOutCommandMenu = new TakeOutFromChestCommandMenu();
            var objsMenu = new ObjsMenu(null, putInCommandMenu, takeOutCommandMenu);

            putIntoChestMenu = objsMenu.PutIntoChest;
            takeOutFromChestMenu = objsMenu.TakeOutFromChest;
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            var player = componentManager.Player;
            if (keyword == StdKw.TakeOutFromChest && obj is RogueObj takeChest)
            {
                // チェストからアイテムを取り出す
                componentManager.EventManager.AddMenu(takeOutFromChestMenu, player, null, new(targetObj: takeChest));
                return true;
            }
            if (keyword == StdKw.PutIntoChest && obj is RogueObj putChest)
            {
                // チェストへアイテムを入れる
                componentManager.EventManager.AddMenu(putIntoChestMenu, player, null, new(targetObj: putChest));
                return true;
            }
            return false;
        }
    }
}
