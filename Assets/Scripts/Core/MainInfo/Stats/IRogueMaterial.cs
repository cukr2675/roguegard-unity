namespace Roguegard
{
    public interface IRogueMaterial : IRogueDescribable
    {
        void AffectValue(EffectableValue value, RogueObj self);
    }
}
