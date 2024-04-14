using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    public class GoalDownStairsBeApplied : BaseApplyRogueMethod
    {
        private static readonly ResultRogueMenu resultRogueMenu = new ResultRogueMenu();

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

        private class ResultRogueMenu : IModelsMenu
        {
            public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
            {
                var dungeon = player.Location;
                var summary = (IResultMenuView)root.Get(DeviceKw.MenuSummary);
                summary.OpenView(ChoiceListPresenter.Instance, Spanning<object>.Empty, root, player, user, arg);
                summary.SetResult(player, dungeon);
            }
        }
    }
}
