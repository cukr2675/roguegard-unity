namespace Roguegard
{
    public class PutObjCommand : CallingObjCommand
    {
        public override string Name => MainInfoKw.Put.Name;

        public override bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var keyword = MainInfoKw.Put;
            var putMethod = self.Main.InfoSet.Put;
            EnqueueMessageRule(self, keyword);
            return RogueMethodAspectState.Invoke(keyword, putMethod, self, user, activationDepth, arg);
        }

        public override ISkillDescribable GetSkillDescribable(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
