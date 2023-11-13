using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class MedicalHerbBeEaten : BaseBeEatenRogueMethod
    {
        private MedicalHerbBeEaten() { }

        public override IRogueMethodTarget Target => WoundedPartyMemberRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => UserRogueMethodRange.Instance;

        protected override void BeEaten(RogueObj self, RogueObj user, float activationDepth)
        {
            using var value = AffectableValue.Get();
            value.Initialize(5f);
            value.SubValues[StdKw.Heal] = 1f;

            this.Hurt(user, self, activationDepth, value);
        }
    }
}
