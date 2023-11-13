using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    public class GoalDownStairsBeApplied : BaseApplyRogueMethod
    {
        private static readonly ResultRogueMenu resultRogueMenu = new ResultRogueMenu();

        public override bool Invoke(RogueObj self, RogueObj player, float activationDepth, in RogueMethodArgument arg)
        {
            if (player != RogueDevice.Primary.Player) return false;

            player.Main.Stats.Direction = RogueDirection.Down;
            RogueDevice.Add(DeviceKw.EnqueueSE, DeviceKw.GameClear);
            RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateBoneMotion(player, CoreMotions.FullTurn, false));
            RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateBoneMotion(player, KeywordBoneMotion.Wait, true));
            RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateBoneMotion(player, CoreMotions.FullTurn, false));
            RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateBoneMotion(player, KeywordBoneMotion.Victory, true));
            RogueDevice.Add(DeviceKw.EnqueueWaitSeconds, 1f);
            RogueDevice.Primary.AddMenu(resultRogueMenu, player, null, RogueMethodArgument.Identity);
            return false;
        }

        private class ResultRogueMenu : IModelsMenu
        {
            public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
            {
                var dungeon = player.Location;
                var summary = (IResultMenuView)root.Get(DeviceKw.MenuSummary);
                summary.OpenView(ChoicesModelsMenuItemController.Instance, Spanning<object>.Empty, root, player, user, arg);
                summary.SetResult(player, dungeon);
            }
        }
    }
}
