namespace Roguegard
{
    public abstract class RogueMaterialAsset : RogueDescriptionAsset, IRogueMaterial
    {
        public abstract void AffectValue(EffectableValue value, RogueObj self);
    }
}
