using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommandRead : CommandRogueMethod
    {
        public override IKeyword Keyword => StdKw.Read;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;
            if (CommonAssert.RequireTool(CategoryKw.Readable, arg, out var tool, out var beAppliedMethod)) return false;

            return RogueMethodAspectState.Invoke(MainInfoKw.BeApplied, beAppliedMethod, tool, self, activationDepth, arg);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
