namespace Roguegard
{
    public abstract class RogueGender : RogueDescriptionData, IRogueGender
    {
        public abstract void AffectValue(EffectableValue value, RogueObj self, MainInfoSetType infoSetType);
    }
}
