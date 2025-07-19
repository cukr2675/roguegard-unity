namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IRogueGender : IRogueDescription
    {
        void AffectValue(EffectableValue value, RogueObj self, MainInfoSetType infoSetType);
    }
}
