using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class CommandUnequip : CommandRogueMethod
    {
        public override IKeyword Keyword => MainInfoKw.Unequip;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.RequireTool(arg, out var tool)) return false;
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;

            var result = this.TryUnequip(tool, self, activationDepth);
            return result;
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
