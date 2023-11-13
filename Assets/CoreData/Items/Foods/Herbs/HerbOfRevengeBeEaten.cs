using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class HerbOfRevengeBeEaten : BaseBeEatenRogueMethod
    {
        private HerbOfRevengeBeEaten() { }

        public override IRogueMethodTarget Target => ForPartyMemberRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => UserRogueMethodRange.Instance;

        protected override void BeEaten(RogueObj self, RogueObj user, float activationDepth)
        {
            this.Affect(user, activationDepth, AutoCounterStatusEffect.Callback, self);
        }
    }
}
