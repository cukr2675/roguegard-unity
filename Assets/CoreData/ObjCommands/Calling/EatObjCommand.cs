namespace Roguegard
{
    public class EatObjCommand : CallingObjCommand
    {
        public override string Name => MainInfoKw.Eat.Name;

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
