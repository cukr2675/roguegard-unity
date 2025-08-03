namespace Roguegard
{
    public class PickUpObjCommand : CallingObjCommand
    {
        public override string Name => MainInfoKw.PickUp.Name;

        public override bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var keyword = MainInfoKw.PickUp;
            var pickUpMethod = self.Main.InfoSet.PickUp;
            EnqueueMessageRule(self, keyword);
            return RogueMethodAspectState.Invoke(keyword, pickUpMethod, self, user, activationDepth, arg);
        }

        public override ISkillDescribable GetSkillDescribable(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
