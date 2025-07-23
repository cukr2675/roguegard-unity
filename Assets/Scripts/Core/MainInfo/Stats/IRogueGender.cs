namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IRogueGender : IRogueDescribable
    {
        void AffectValue(EffectableValue value, RogueObj self, MainInfoSetType infoSetType);
    }
}
