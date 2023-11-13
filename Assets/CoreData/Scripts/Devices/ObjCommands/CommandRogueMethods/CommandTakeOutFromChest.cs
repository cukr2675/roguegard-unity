using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommandTakeOutFromChest : CommandRogueMethod
    {
        public override IKeyword Keyword => StdKw.TakeOutFromChest;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;
            if (arg.Tool.Main.InfoSet.Category != CategoryKw.Chest) return false;

            var chestInfo = ChestInfo.GetInfo(arg.Tool);
            if (chestInfo == null) return false;

            var takeOutArg = new RogueMethodArgument(count: 0);
            return RogueMethodAspectState.Invoke(MainInfoKw.BeApplied, chestInfo.BeOpened, arg.Tool, self, activationDepth, takeOutArg);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
