namespace Roguegard
{
    public abstract class RogueMaterial : RogueDescriptionData, IRogueMaterial
    {
        public abstract void AffectValue(EffectableValue value, RogueObj self);
    }
}
