namespace Roguegard
{
    public class HerbOfGainLevelBeEaten : BaseBeEatenRogueMethod
    {
        private HerbOfGainLevelBeEaten() { }

        public override IRogueMethodTarget Target => ForPartyMemberRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => UserRogueMethodRange.Instance;

        private static readonly LoseExpRogueMethod loseExp = new();

        protected override void BeEaten(RogueObj self, RogueObj user, float activationDepth)
        {
            // レベルアップ
            RogueMethodAspectState.Invoke(StdKw.LoseExp, loseExp, self, user, activationDepth, RogueMethodArgument.Identity);
        }

        private class LoseExpRogueMethod : IChangeStateRogueMethod
        {
            public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                user.Main.Stats.SetLv(user, user.Main.Stats.Lv + 1);
                return true;
            }
        }
    }
}
