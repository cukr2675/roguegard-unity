using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class EatObjCommand : BaseObjCommand
    {
        public override string Name => MainInfoKw.Eat.Name;

        public override bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var keyword = MainInfoKw.Eat;
            var eatMethod = self.Main.InfoSet.Eat;
            EnqueueMessageRule(keyword);
            return RogueMethodAspectState.Invoke(keyword, eatMethod, self, user, activationDepth, arg);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return tool?.Main.InfoSet.BeEaten;
        }
    }
}
