namespace Roguegard
{
    public interface IWeightedRogueObjGenerator : IRogueObjGenerator
    {
        float Weight { get; }
    }
}
