using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommandPutIntoContainer : CommandRogueMethod
    {
        public override IKeyword Keyword => StdKw.PutIntoContainer;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;
            if (arg.Tool.Main.InfoSet.Category != CategoryKw.Container) return false;

            var containerInfo = ContainerInfo.GetInfo(arg.Tool);
            if (containerInfo == null) return false;

            var putInArg = new RogueMethodArgument(count: 1);
            return RogueMethodAspectState.Invoke(MainInfoKw.BeApplied, containerInfo.BeOpened, arg.Tool, self, activationDepth, putInArg);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
