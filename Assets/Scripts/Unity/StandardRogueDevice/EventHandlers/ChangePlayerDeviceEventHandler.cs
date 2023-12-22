using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class ChangePlayerDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly StandardRogueDeviceComponentManager componentManager;
        private readonly TouchController touchController;
        private readonly RogueTicker ticker;
        private readonly System.Action<RogueObj> setPlayer;

        public ChangePlayerDeviceEventHandler(
            StandardRogueDeviceComponentManager componentManager, TouchController touchController, RogueTicker ticker,
            System.Action<RogueObj> setPlayer)
        {
            this.componentManager = componentManager;
            this.touchController = touchController;
            this.ticker = ticker;
            this.setPlayer = setPlayer;
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            if (keyword == DeviceKw.ChangePlayer && obj is RogueObj newPlayer)
            {
                OnChangePlayer(componentManager.Player, newPlayer);
                return true;
            }
            return false;
        }

        private void OnChangePlayer(RogueObj player, RogueObj newPlayer)
        {
            // ViewInfo を移動させる
            var view = player.Get<ViewInfo>();
            player.RemoveInfo(typeof(ViewInfo));
            newPlayer.SetInfo(view);

            // PlayerLeaderInfo を移動させる
            var playerLeaderInfo = player.Main.GetPlayerLeaderInfo(player);
            playerLeaderInfo?.Move(player, newPlayer);

            // RogueDeviceEffect を移動させる
            var deviceEffect = RogueDeviceEffect.Get(player);
            deviceEffect.RemoveClose(player);
            RogueDeviceEffect.SetTo(newPlayer);

            // プレイヤーキャラを変更
            setPlayer(newPlayer);

            // 変更後の初期化処理
            touchController.OpenWalker(newPlayer);
            touchController.MenuOpen(newPlayer);
            ticker.Reset();
            componentManager.UpdateCharacters();
        }
    }
}
