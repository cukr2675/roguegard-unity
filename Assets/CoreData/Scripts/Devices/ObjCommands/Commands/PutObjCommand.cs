using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class PutObjCommand : BaseObjCommand
    {
        public override string Name => MainInfoKw.Put.Name;

        public override bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var keyword = MainInfoKw.Put;
            var putMethod = self.Main.InfoSet.Put;
            EnqueueMessageRule(keyword);
            return RogueMethodAspectState.Invoke(keyword, putMethod, self, user, activationDepth, arg);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
