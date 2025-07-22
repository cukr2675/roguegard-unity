namespace Roguegard
{
    public abstract class RogueGenderAsset : RogueDescriptionAsset, IRogueGender
    {
        public abstract void AffectValue(EffectableValue value, RogueObj self, MainInfoSetType infoSetType);
    }
}
