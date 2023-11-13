using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommandPutIntoChest : CommandRogueMethod
    {
        public override IKeyword Keyword => StdKw.PutIntoChest;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;
            if (arg.Tool.Main.InfoSet.Category != CategoryKw.Chest) return false;

            var chestInfo = ChestInfo.GetInfo(arg.Tool);
            if (chestInfo == null) return false;

            var putInArg = new RogueMethodArgument(count: 1);
            return RogueMethodAspectState.Invoke(MainInfoKw.BeApplied, chestInfo.BeOpened, arg.Tool, self, activationDepth, putInArg);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
