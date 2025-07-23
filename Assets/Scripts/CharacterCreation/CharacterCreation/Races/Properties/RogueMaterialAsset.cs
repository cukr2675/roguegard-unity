namespace Roguegard
{
    public abstract class RogueMaterialAsset : RogueDescribableAsset, IRogueMaterial
    {
        public abstract void AffectValue(EffectableValue value, RogueObj self);
    }
}
