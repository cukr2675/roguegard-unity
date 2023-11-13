using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class HerbOfPaintBeThrown : BaseApplyRogueMethod
    {
        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.BeThrown(
                LineOfSight10RogueMethodRange.Instance, self, user, arg,
                out var target, out var hitPosition, out var from, out var to, out var raycasted)) return false;
            if (raycasted)
            {
                MainCharacterWorkUtility.TryAddBeThrown(self, user, hitPosition, from, CoreMotions.BeThrownFlying);
            }

            if (target != null && activationDepth < 1f)
            {
                var eatMethod = target.Main.InfoSet.Eat;
                var eatArg = new RogueMethodArgument(tool: self);
                var eatResult = RogueMethodAspectState.Invoke(MainInfoKw.Eat, eatMethod, target, user, 1f, eatArg);
                if (eatResult) return true;
            }

            // 食べられなかった場合は地面に着弾してペイントトラップになる。
            MainCharacterWorkUtility.TryAddBeDropped(self, user, to, CoreMotions.BeThrownDrop);
            var tile = new UserRogueTile(CoreTiles1.PaintTrap, user);
            user.Location.Space.TrySet(tile, to);

            // スタックしていたら一つだけ消費する。
            self.TrySetStack(self.Stack - 1);
            return true;
        }
    }
}
