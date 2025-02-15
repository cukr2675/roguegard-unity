using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class BaseApplyRogueMethod : ReferableScript, IApplyRogueMethod
    {
        public virtual IRogueMethodTarget Target => null;
        public virtual IRogueMethodRange Range => null;
        int ISkillDescription.RequiredMp => 0;
        Spanning<IKeyword> ISkillDescription.AmmoCategories => Spanning<IKeyword>.Empty;

        public abstract bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);

        int ISkillDescription.GetAtk(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }
    }
}
