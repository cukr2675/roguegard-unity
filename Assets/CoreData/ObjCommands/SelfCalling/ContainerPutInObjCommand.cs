namespace Roguegard
{
    public class ContainerPutInObjCommand : SelfCallingObjCommand
    {
        public override IKeyword Keyword => StdKw.PutIntoContainer;

        protected override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;
            if (arg.Tool.Main.InfoSet.Category != CategoryKw.Container) return false;

            var containerInfo = ContainerInfo.GetInfo(arg.Tool);
            if (containerInfo == null) return false;

            var putInArg = new RogueMethodArgument(count: 1);
            return RogueMethodAspectState.Invoke(MainInfoKw.BeApplied, containerInfo.BeOpened, arg.Tool, self, activationDepth, putInArg);
        }

        public override ISkillDescribable GetSkillDescribable(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
