using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class HerbOfSleepingBeEaten : BaseBeEatenRogueMethod
    {
        private HerbOfSleepingBeEaten() { }

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => UserRogueMethodRange.Instance;

        protected override void BeEaten(RogueObj self, RogueObj user, float activationDepth)
        {
            this.Affect(user, activationDepth, SleepStatusEffect.Callback, self);
        }
    }
}
