using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class FoodRace : ObjectRace
    {
        public override IApplyRogueMethod BeThrown
            => base.BeThrown != RoguegardSettings.DefaultRaceOption.BeThrown ? base.BeThrown : (virtualBeThrown ??= new BeThrownRogueMethod(this));

        private BeThrownRogueMethod virtualBeThrown;

        private class BeThrownRogueMethod : BaseApplyRogueMethod
        {
            private readonly FoodRace parent;

            public override IRogueMethodTarget Target => parent.BeEaten.Target;
            public override IRogueMethodRange Range => LineOfSight10RogueMethodRange.Instance;

            public BeThrownRogueMethod(FoodRace parent)
            {
                this.parent = parent;
            }

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

                // 食べられなかった場合は普通に投げられる。
                MainCharacterWorkUtility.TryAddBeDropped(self, user, to, CoreMotions.BeThrownDrop);

                // スタックしていたら一つだけ投げる。
                if (self.Stack >= 2) { self = SpaceUtility.Divide(self, 1); }
                this.Locate(self, user, user.Location, to, activationDepth);
                return true;
            }
        }
    }
}
