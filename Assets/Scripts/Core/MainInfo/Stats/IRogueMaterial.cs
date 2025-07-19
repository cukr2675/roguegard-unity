namespace Roguegard
{
    public interface IRogueMaterial : IRogueDescription
    {
        void AffectValue(EffectableValue value, RogueObj self);
    }
}
