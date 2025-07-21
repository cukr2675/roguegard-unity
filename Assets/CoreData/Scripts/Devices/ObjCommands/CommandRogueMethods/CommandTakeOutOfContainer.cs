namespace Roguegard
{
    public class CommandTakeOutOfContainer : CommandRogueMethod
    {
        public override IKeyword Keyword => StdKw.TakeOutOfContainer;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;
            if (arg.Tool.Main.InfoSet.Category != CategoryKw.Container) return false;

            var containerInfo = ContainerInfo.GetInfo(arg.Tool);
            if (containerInfo == null) return false;

            var takeOutArg = new RogueMethodArgument(count: 0);
            return RogueMethodAspectState.Invoke(MainInfoKw.BeApplied, containerInfo.BeOpened, arg.Tool, self, activationDepth, takeOutArg);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
