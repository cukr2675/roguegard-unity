using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class HerbOfPaintBeEaten : BaseBeEatenRogueMethod
    {
        private HerbOfPaintBeEaten() { }

        public override IRogueMethodTarget Target => ForPartyMemberRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => UserRogueMethodRange.Instance;

        protected override void BeEaten(RogueObj self, RogueObj user, float activationDepth)
        {
            this.Affect(user, activationDepth, PaintStatusEffect.Callback, self);
        }
    }
}
