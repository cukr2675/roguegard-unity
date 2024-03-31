using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class ThrowObjCommand : BaseObjCommand
    {
        public override string Name => MainInfoKw.Throw.Name;

        public override bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var keyword = MainInfoKw.Throw;
            var throwMethod = self.Main.InfoSet.Throw;
            EnqueueMessageRule(self, keyword);
            return RogueMethodAspectState.Invoke(keyword, throwMethod, self, user, activationDepth, arg);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return self.Main.InfoSet.Throw;
        }
    }
}
