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
        private readonly IListMenu putIntoChestMenu;
        private readonly IListMenu takeOutFromChestMenu;

        public ChestDeviceEventHandler(
            StandardRogueDeviceComponentManager componentManager, IListMenu putIntoChestMenu, IListMenu takeOutFromChestMenu)
        {
            this.componentManager = componentManager;
            this.putIntoChestMenu = putIntoChestMenu;
            this.takeOutFromChestMenu = takeOutFromChestMenu;
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            var player = componentManager.Player;
            if (keyword == StdKw.TakeOutFromChest && obj is RogueObj takeChest)
            {
                // �`�F�X�g����A�C�e�������o��
                componentManager.EventManager.AddMenu(takeOutFromChestMenu, player, null, new(targetObj: takeChest));
                return true;
            }
            if (keyword == StdKw.PutIntoChest && obj is RogueObj putChest)
            {
                // �`�F�X�g�փA�C�e��������
                componentManager.EventManager.AddMenu(putIntoChestMenu, player, null, new(targetObj: putChest));
                return true;
            }
            return false;
        }
    }
}
