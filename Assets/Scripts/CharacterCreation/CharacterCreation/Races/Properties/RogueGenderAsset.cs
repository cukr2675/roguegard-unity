namespace Roguegard
{
    public abstract class RogueGenderAsset : RogueDescribableAsset, IRogueGender
    {
        public abstract void AffectValue(EffectableValue value, RogueObj self, MainInfoSetType infoSetType);
    }
}
