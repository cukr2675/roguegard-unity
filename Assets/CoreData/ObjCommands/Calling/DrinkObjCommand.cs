namespace Roguegard
{
    public class DrinkObjCommand : CallingObjCommand
    {
        public override string Name => "飲む";

        public override bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var keyword = MainInfoKw.Eat;
            var eatMethod = self.Main.InfoSet.Eat;
            EnqueueMessageRule(self, keyword);
            return RogueMethodAspectState.Invoke(keyword, eatMethod, self, user, activationDepth, arg);
        }

        public override ISkillDescribable GetSkillDescribable(RogueObj self, RogueObj tool)
        {
            return tool?.Main.InfoSet.BeEaten;
        }
    }
}
