using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommandDownStairs : CommandRogueMethod
    {
        public override IKeyword Keyword => CategoryKw.DownStairs;

        public override string Name => "下りる";

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.RequireTool(CategoryKw.DownStairs, arg, out var tool, out var beAppliedMethod)) return false;

            var result = RogueMethodAspectState.Invoke(CategoryKw.DownStairs, beAppliedMethod, tool, self, activationDepth, arg);
            return result;
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
