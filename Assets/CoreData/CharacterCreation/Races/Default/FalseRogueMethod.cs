using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class FalseRogueMethod : MpSkill, IApplyRogueMethod, IAffectRogueMethod, IChangeStateRogueMethod, IChangeEffectRogueMethod
    {
        private FalseRogueMethod() { }

        public override string Name => "None";
        public override IRogueMethodTarget Target => null;
        public override IRogueMethodRange Range => null;
        public override int RequiredMp => 0;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            return false;
        }
    }
}
