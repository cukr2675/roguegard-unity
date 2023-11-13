using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class BaseApplyRogueMethod : ReferableScript, IApplyRogueMethod
    {
        public virtual IRogueMethodTarget Target => null;
        public virtual IRogueMethodRange Range => null;
        int ISkillDescription.RequiredMP => 0;
        Spanning<IKeyword> ISkillDescription.AmmoCategories => Spanning<IKeyword>.Empty;

        public abstract bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);

        int ISkillDescription.GetATK(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }
    }
}
