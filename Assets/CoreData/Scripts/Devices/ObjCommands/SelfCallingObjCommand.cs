namespace Roguegard
{
    /// <summary>
    /// このクラス自体が <see cref="IRogueMethod"/> になる選択肢モデルクラス
    /// </summary>
    public abstract class SelfCallingObjCommand : CallingObjCommand, IActiveRogueMethod
    {
        public abstract IKeyword Keyword { get; }

        public override string Name => Keyword.Name;

        public override bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            EnqueueMessageRule(self, Keyword);
            return RogueMethodAspectState.Invoke(Keyword, this, self, user, activationDepth, arg);
        }

        protected abstract bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);

        bool IRogueMethod.Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            return Invoke(self, user, activationDepth, arg);
        }
    }
}
