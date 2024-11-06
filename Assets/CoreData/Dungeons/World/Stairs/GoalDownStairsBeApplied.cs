using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    public class GoalDownStairsBeApplied : BaseApplyRogueMethod
    {
        private static readonly ResultRogueMenu resultRogueMenu = new();

        public override bool Invoke(RogueObj self, RogueObj player, float activationDepth, in RogueMethodArgument arg)
        {
            player.Main.Stats.Direction = RogueDirection.Down;
            if (MessageWorkListener.TryOpenHandler(player.Location, player.Position, out var h))
            {
                using var handler = h;
                handler.EnqueueSE(DeviceKw.GameClear);
                handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(player, CoreMotions.FullTurn, false));
                handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(player, KeywordSpriteMotion.Wait, true));
                handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(player, CoreMotions.FullTurn, false));
                handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(player, KeywordSpriteMotion.Victory, true));
                handler.Handle(DeviceKw.EnqueueWaitSeconds, 1f);
            }

            if (player == RogueDevice.Primary.Player)
            {
                // プレイヤー操作を要求するため、プレイヤーキャラのみ実行可能とする
                RogueDevice.Primary.AddMenu(resultRogueMenu, player, null, RogueMethodArgument.Identity);
                return false;
            }
            else if (activationDepth < 1f)
            {
                var info = RogueWorldSavePointInfo.Instance;
                if (!default(IActiveRogueMethodCaller).LocateSavePoint(player, self, 1f, info)) return false;

                var memberInfo = LobbyMemberList.GetMemberInfo(player);
                memberInfo.SavePoint = info;
                return false;
            }
            return false;
        }

        private class ResultRogueMenu : RogueMenuScreen
        {
            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var player = arg.Self;
                var dungeon = player.Location;
                manager.SetResult(player, dungeon);
            }
        }
    }
}
