namespace Roguegard
{
    public interface IAffectCallback
    {
        IRogueEffect AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg);
    }
}
