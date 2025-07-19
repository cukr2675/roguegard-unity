namespace Roguegard
{
    public interface IRogueMethodTarget : IRogueDescription
    {
        IRoguePredicator GetPredicator(RogueObj self, float predictionDepth, RogueObj tool);
    }
}
