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
        private readonly IModelsMenu openChestMenu;

        public ChestDeviceEventHandler(StandardRogueDeviceComponentManager componentManager, IModelsMenu openChestMenu)
        {
            this.componentManager = componentManager;
            this.openChestMenu = openChestMenu;
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            var player = componentManager.Player;
            if (keyword == StdKw.TakeOutFromChest && obj is RogueObj takeChest)
            {
                // �`�F�X�g����A�C�e�������o��
                componentManager.EventManager.AddMenu(openChestMenu, player, null, new(targetObj: takeChest, count: 0));
                return true;
            }
            if (keyword == StdKw.PutIntoChest && obj is RogueObj putChest)
            {
                // �`�F�X�g�փA�C�e��������
                componentManager.EventManager.AddMenu(openChestMenu, player, null, new(targetObj: putChest, count: 1));
                return true;
            }
            return false;
        }
    }
}
