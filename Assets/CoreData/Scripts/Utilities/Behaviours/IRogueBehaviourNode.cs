namespace Roguegard
{
    public interface IRogueBehaviourNode
    {
        RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth);
    }
}
