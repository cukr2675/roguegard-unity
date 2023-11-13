using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class HerbOfVisionBeEaten : BaseBeEatenRogueMethod
    {
        private HerbOfVisionBeEaten() { }

        public override IRogueMethodTarget Target => null;
        public override IRogueMethodRange Range => UserRogueMethodRange.Instance;

        protected override void BeEaten(RogueObj self, RogueObj user, float activationDepth)
        {
            this.Affect(user, activationDepth, VisionStatusEffect.Callback, self);
        }
    }
}
