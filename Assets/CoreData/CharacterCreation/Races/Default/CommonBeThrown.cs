using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class CommonBeThrown : BaseApplyRogueMethod
    {
        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RaycastAssert.BeThrown(
                LineOfSight10RogueMethodRange.Instance, self, user, arg,
                out _, out var hitPosition, out var from, out var to, out var raycasted)) return false;
            if (raycasted)
            {
                MainCharacterWorkUtility.TryAddBeThrown(self, user, hitPosition, from, CoreMotions.BeThrownFlying);
            }
            MainCharacterWorkUtility.TryAddBeDropped(self, user, to, CoreMotions.BeThrownDrop);

            // スタックしていたら一つだけ投げる。
            if (self.Stack >= 2) { self = SpaceUtility.Divide(self, 1); }
            this.Locate(self, user, user.Location, to, activationDepth);
            return true;
        }
    }
}
