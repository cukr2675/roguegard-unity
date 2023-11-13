using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class PaintTrapBeApplied : BaseApplyRogueMethod
    {
        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (arg.Other is UserRogueTile userTile && !StatsEffectedValues.AreVS(userTile.User, user))
            {
                // 敵対していないキャラが罠を踏んでも起動しないようにする
                return false;
            }

            if (RogueDevice.Primary.VisibleAt(user.Location, user.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, ":PaintTrapMsg::2");
                RogueDevice.Add(DeviceKw.AppendText, user);
                RogueDevice.Add(DeviceKw.AppendText, CoreTiles1.PaintTrap);
                RogueDevice.Add(DeviceKw.AppendText, "\n");
            }
            return this.TryAffect(user, activationDepth, PaintStatusEffect.Callback);
        }
    }
}
