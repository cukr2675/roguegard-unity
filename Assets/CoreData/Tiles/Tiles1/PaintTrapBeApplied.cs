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

            if (MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(":PaintTrapMsg::2").AppendText(user).AppendText(CoreTiles1.PaintTrap).AppendText("\n");
            }
            return this.TryAffect(user, activationDepth, PaintStatusEffect.Callback);
        }
    }
}
