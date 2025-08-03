using Roguegard.Extensions;

namespace Roguegard
{
    public class UnEquipObjCommand : SelfCallingObjCommand
    {
        public override IKeyword Keyword => MainInfoKw.Unequip;

        protected override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.RequireTool(arg, out var tool)) return false;
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;

            var result = this.TryUnequip(tool, self, activationDepth);
            return result;
        }

        public override ISkillDescribable GetSkillDescribable(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
