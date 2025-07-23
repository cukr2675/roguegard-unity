namespace Roguegard
{
    public interface IRogueMethodTarget : IRogueDescribable
    {
        IRoguePredicator GetPredicator(RogueObj self, float predictionDepth, RogueObj tool);
    }
}
