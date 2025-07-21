namespace Roguegard
{
    public interface IClosableStatusEffect : IStatusEffect
    {
        void RemoveClose(RogueObj self);
    }
}
